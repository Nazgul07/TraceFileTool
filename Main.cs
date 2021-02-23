using System;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace TraceFileTool
{
    public partial class Main : Form
    {
        private TreeNode<Message> _root = new TreeNode<Message>(new Message());
        private TreeNode<Message> _current = null;
        private bool RequestServerRequestsPresent = false;
        public Main()
        {
            InitializeComponent();
        }
        private void btMine_Click(object sender, EventArgs e)
        {
            RequestServerRequestsPresent = false;
            _root = new TreeNode<Message>(new Message()); 
            _current = null;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    string file = tbFile.Text;
                    using (StreamReader reader = new StreamReader(file))
                    using (StreamWriter writer = new StreamWriter(file + ".csv"))
                    {
                        long length = 0L;
                        progress.Invoke(new Action(() => { progress.Visible = true; }));
                        StringBuilder sb = new StringBuilder();
                        while (!reader.EndOfStream)
                        {
                            length += 1;
                            if (length % 100L == 0L)
                            {
                                double percent = (double)reader.BaseStream.Position / (double)reader.BaseStream.Length * 100.0;
                                progress.Invoke(new Action(() => { progress.Value = (int)(percent * 10.0); }));
                            }
                            string line = reader.ReadLine();
                            if (line.StartsWith("<mdebug") && !line.Contains("</mdebug"))
                            {
                                sb.Append(line);
                                continue;
                            }
                            else if (line.Contains("</mdebug"))
                            {
                                sb.Append(line);
                                line = sb.ToString();
                                sb.Clear();
                            }
                            else if (sb.Length > 0)
                            {
                                sb.Append(line);
                                continue;
                            }
                            try
                            {
                                if (line.StartsWith("<"))
                                {
                                    ProcessMessage(line);
                                }
                            }
                            catch
                            {
                                line = line.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
                                var match = Regex.Match(line, "(?<=value=\").*(?=\"/>)");
                                if (match.Value.Length == 0)
                                {
                                    continue;
                                }
                                line = line.Replace(match.Value, SecurityElement.Escape(match.Value));
                                ProcessMessage(line);
                            }
                        }
                        progress.Invoke(new Action(() => { progress.Visible = false; }));
                        writer.WriteLine("Name, Times Called, Average Milliseconds (with children), Average Milliseconds (without children), Total Milliseconds (with children), Total Milliseconds (without children), Max Single Milliseconds (with children), Max Single Milliseconds (without children)");
                        var fieldGroups = _root.FlattenChildren().Where(x =>
                        {
                            if(RequestServerRequestsPresent && x.Value.IsRequest && x.Value.RequestFromManuScript)
                            {
                                return false;
                            }
                            if (cbExcludeInternals.Checked && !x.Value.DisplayName.Contains('.'))
                            {
                                return false;
                            }
                            return x.Value.End > DateTime.MinValue;
                        }).GroupBy(x => x.Value.DisplayName);
                        
                        foreach (IGrouping<string, TreeNode<Message>> group in fieldGroups)
                        {
                            var total = group.Sum(x => x.Value.Total.TotalMilliseconds);
                            double exclusive = 0;
                            foreach (var item in group)
                            {
                                exclusive += item.Children.Where(x =>
                                {
                                    return x.Value.End > DateTime.MinValue;
                                }).Sum(x => x.Value.Total.TotalMilliseconds);
                            }
                            exclusive = total - exclusive;
                            var avg = Math.Round(group.Average(x => x.Value.Total.TotalMilliseconds));
                            var totalCalls = group.Count();
                            var avgWithoutChildren = Math.Round(exclusive / totalCalls);
                            string name = group.Key;
                            var max = group.Max(x => x.Value.Total.TotalMilliseconds);
                            var maxItem = group.First(x => x.Value.Total.TotalMilliseconds == max);
                            var maxWithoutChildren = max - maxItem.Children.Where(x =>
                            {
                                return x.Value.End > DateTime.MinValue;
                            }).Sum(x => x.Value.Total.TotalMilliseconds);
                            writer.WriteLine($"{name},{totalCalls},{avg},{avgWithoutChildren},{total},{exclusive},{max},{maxWithoutChildren}");
                        }
                    }
                    Process.Start(file + ".csv");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void ProcessMessage(string message)
        {
            if (message.StartsWith("<mdebug") || message.StartsWith("<request") || message.StartsWith("<response"))
            {
                var xml = XElement.Parse(message);
                DateTime dateTime = DateTime.ParseExact(xml.Attribute("time").Value, "yyyy-MM-ddTHH:mm:ss:fff", CultureInfo.InvariantCulture);
                if (xml.Name == "request")
                {
                    RequestServerRequestsPresent = true;
                    XElement field = xml.XPathSelectElement("requestMade");
                    long debugId = long.Parse(xml.Value);
                    string name = xml.Attribute("messageText").Value;
                    if (!name.EndsWith("Rq")) return;
                    var parent = _current ?? _root;
                    _current = new TreeNode<Message>(new Message() { DebugId = debugId, DisplayName = name, Start = dateTime, IsRequest = true });
                    parent.AddChild(_current);
                }
                else if (xml.Name == "response")
                {
                    long debugId = long.Parse(xml.Value);
                    string name = xml.Attribute("messageText").Value;
                    if (!name.EndsWith("Rs")) return;
                    if (_current?.Value.DebugId == debugId)
                    {
                        Message item = _current.Value;
                        item.End = dateTime;
                        _current = _current.Parent;
                    }
                    else
                    {
                        if (_current != null && _current.Parents.Any(x => x.Value.DebugId == debugId))
                        {
                            _current = _current.Parents.First(x => x.Value.DebugId == debugId);
                            Message item = _current.Value;
                            item.End = dateTime;
                            _current = _current?.Parent;
                        }
                    }
                }
                else if (xml.Name == "mdebug" && xml.XPathSelectElement("requestMade") != null)
                {
                    long level = long.Parse((xml.FirstNode as XElement).Attribute("level").Value);
                    XElement field = xml.XPathSelectElement("requestMade");
                    long debugId = long.Parse(field.Attribute("debugId").Value);
                    string name = XElement.Parse(field.Attribute("xml").Value).Name.LocalName;
                    var parent = _current ?? _root;
                    _current = new TreeNode<Message>(new Message() { DebugId = debugId, DisplayName = name, Start = dateTime, Level = level, IsRequest = true, RequestFromManuScript = true});
                    parent.AddChild(_current);
                }
                else if (xml.Name == "mdebug")
                {
                    XElement field = (xml.FirstNode as XElement);
                    long level = long.Parse(field.Attribute("level").Value);
                    long debugId = long.Parse(field.Attribute("debugId").Value);
                    string id = field.Name.LocalName;
                    if (field.Name == "public" || field.Name == "private")
                    {
                        id = field.Attribute("id").Value;
                    }
                    var parent = _current ?? _root;
                    _current = new TreeNode<Message>(new Message() { DebugId = debugId, DisplayName = id, Start = dateTime, Level = level });
                    parent.AddChild(_current);
                }
                else if (xml.Name == "mdebugr")
                {
                    XElement field = xml.XPathSelectElement("result");
                    long debugId = long.Parse(field.Attribute("debugId").Value);
                    if (_current?.Value.DebugId == debugId)
                    {
                        Message item = _current.Value;
                        item.End = dateTime;
                        _current = _current.Parent;
                    }
                    else
                    {
                        if(_current != null && _current.Parents.Any(x => x.Value.DebugId == debugId))
                        {
                            _current = _current.Parents.First(x => x.Value.DebugId == debugId);
                            Message item = _current.Value;
                            item.End = dateTime;
                            _current = _current?.Parent;
                        }
                    }
                }
            }
        }


        private void btGo_Click(object sender, EventArgs e)
        {
            string file = tbFile.Text;
            string fileOut = Path.Combine(Path.GetDirectoryName(file), "Fixed-" + Path.GetFileName(file));
            using (StreamReader reader = new StreamReader(file))
            using (StreamWriter writer = new StreamWriter(fileOut))
            {
                long length = 0L;
                progress.Visible = true;
                while (!reader.EndOfStream)
                {
                    length += 1;
                    if (length % 100L == 0L)
                    {
                        double percent = (double)reader.BaseStream.Position / (double)reader.BaseStream.Length * 100.0;
                        progress.Value = (int)(percent * 10.0);
                    }
                    string line = reader.ReadLine();
                    try
                    {
                        if (line.StartsWith("<"))
                        {
                            XElement.Parse(line);
                            writer.WriteLine(line);
                        }
                        else
                        {
                            writer.WriteLine(line);
                        }
                    }
                    catch
                    {
                        line = line.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "");
                        line = Regex.Replace(line, "<Session.loginRq.*?/>", "<Session.loginRq/>");
                        var match = Regex.Match(line, "(?<=value=\").*(?=\"/>)");
                        if(match.Value.Length == 0)
                        {
                            writer.WriteLine(line);
                            continue;
                        }
                        line = line.Replace(match.Value, SecurityElement.Escape(match.Value));
                        writer.WriteLine(line);
                    }
                }
                MessageBox.Show("Fixed file written to: "+fileOut);
                progress.Visible = false;
            }
        }

        private void btBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog diag = new OpenFileDialog())
            {
                diag.ShowDialog();
                tbFile.Text = diag.FileName;
            }
        }

        private void btnSessions_Click(object sender, EventArgs e)
        {
            Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>();
            string file = tbFile.Text;
            string fileOut = Path.Combine(Path.GetDirectoryName(file), "Fixed-" + Path.GetFileName(file));

            using (MultiStream reader = new MultiStream(new[] { GenerateStreamFromString("<root>"), File.OpenRead(file), GenerateStreamFromString("</root>") }))
            using (XmlReader xmlReader = XmlReader.Create(reader))
            {
                long length = 0L;
                progress.Visible = true;
                while (xmlReader.Read())
                {
                    length += 1;
                    if (length % 100L == 0L)
                    {
                        double percent = (double)reader.Position / (double)reader.Length * 100.0;
                        progress.Value = (int)(percent * 10.0);
                    }
                    if (xmlReader.Name != "root" && xmlReader.NodeType == XmlNodeType.Element)
                    {
                        var sessionid = xmlReader.GetAttribute("sessionId");
                        var xml = xmlReader.ReadOuterXml();
                        if (!string.IsNullOrEmpty(sessionid))
                        {
                            var fileName = Path.Combine(Path.GetDirectoryName(file), $"{sessionid.Replace(":", "-")}-{Path.GetFileName(file)}");
                            StreamWriter writer;
                            if (writers.ContainsKey(fileName))
                            {
                                writer = writers[fileName];
                            }
                            else
                            {
                                writer = new StreamWriter(File.OpenWrite(fileName));
                                writers.Add(fileName, writer);
                            }
                            writer.WriteLine(xml);
                        }
                    }
                }
                foreach (var writer in writers.Values)
                {
                    writer.Close();
                    writer.Dispose();
                }

                progress.Visible = false;
            }
        }
        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
