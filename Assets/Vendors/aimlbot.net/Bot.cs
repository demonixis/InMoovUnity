using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
#if !UNITY_WEBPLAYER && !UNITY_WEBGL
using System.Net.Mail;
#endif
using UnityEngine;
using AIMLbot.Utils;

namespace AIMLbot
{
    /// <summary>
    /// Encapsulates a bot. If no settings.xml file is found or referenced the bot will try to
    /// default to safe settings.
    /// </summary>
    public class Bot
    {
        #region Attributes
        
        /// <summary>
        /// Loads settings based upon the default location of the Settings.xml file
        /// </summary>
        private string _customPath = Application.streamingAssetsPath;


        /// <summary>
        /// A dictionary object that looks after all the settings associated with this bot
        /// </summary>
        public SettingsDictionary GlobalSettings;

        /// <summary>
        /// A dictionary of all the gender based substitutions used by this bot
        /// </summary>
        public SettingsDictionary GenderSubstitutions;

        /// <summary>
        /// A dictionary of all the first person to second person (and back) substitutions
        /// </summary>
        public SettingsDictionary Person2Substitutions;

        /// <summary>
        /// A dictionary of first / third person substitutions
        /// </summary>
        public SettingsDictionary PersonSubstitutions;

        /// <summary>
        /// Generic substitutions that take place during the normalization process
        /// </summary>
        public SettingsDictionary Substitutions;

        /// <summary>
        /// The default predicates to set up for a user
        /// </summary>
        public SettingsDictionary DefaultPredicates;

        /// <summary>
        /// Holds information about the available custom tag handling classes (if loaded)
        /// Key = class name
        /// Value = TagHandler class that provides information about the class
        /// </summary>
        private Dictionary<string, TagHandler> _customTags;

        /// <summary>
        /// Holds references to the assemblies that hold the custom tag handling code.
        /// </summary>
        private Dictionary<string, Assembly> _lateBindingAssemblies = new();

        /// <summary>
        /// An List<> containing the tokens used to split the input into sentences during the 
        /// normalization process
        /// </summary>
        public List<string> Splitters = new();

        /// <summary>
        /// A buffer to hold log messages to be written out to the log file when a max size is reached
        /// </summary>
        private List<string> _logBuffer = new();

        /// <summary>
        /// How big to let the log buffer get before writing to disk
        /// </summary>
        private int MaxLogBufferSize => Convert.ToInt32(GlobalSettings.grabSetting("maxlogbuffersize"));

        /// <summary>
        /// Flag to show if the bot is willing to accept user input
        /// </summary>
        public bool isAcceptingUserInput = true;

        /// <summary>
        /// The message to show if a user tries to use the bot whilst it is set to not process user input
        /// </summary>
        private string NotAcceptingUserInputMessage => GlobalSettings.grabSetting("notacceptinguserinputmessage");

        /// <summary>
        /// The maximum amount of time a request should take (in milliseconds)
        /// </summary>
        public double TimeOut => Convert.ToDouble(GlobalSettings.grabSetting("timeout"));

        /// <summary>
        /// The message to display in the event of a timeout
        /// </summary>
        public string TimeOutMessage => GlobalSettings.grabSetting("timeoutmessage");

        /// <summary>
        /// The locale of the bot as a CultureInfo object
        /// </summary>
        public CultureInfo Locale => new CultureInfo(GlobalSettings.grabSetting("culture"));

        /// <summary>
        /// Will match all the illegal characters that might be inputted by the user
        /// </summary>
        public Regex Strippers =>
            new Regex(GlobalSettings.grabSetting("stripperregex"), RegexOptions.IgnorePatternWhitespace);

        /// <summary>
        /// The email address of the botmaster to be used if WillCallHome is set to true
        /// </summary>
        public string AdminEmail
        {
            get => GlobalSettings.grabSetting("adminemail");
            set
            {
                if (value.Length > 0)
                {
                    // check that the email is valid
                    var patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                                        + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                        + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                        + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                        + @"[a-zA-Z]{2,}))$";
                    var reStrict = new Regex(patternStrict);

                    if (reStrict.IsMatch(value))
                        // update the settings
                        GlobalSettings.addSetting("adminemail", value);
                    else
                        throw new Exception("The AdminEmail is not a valid email address");
                }
                else
                {
                    GlobalSettings.addSetting("adminemail", "");
                }
            }
        }

        /// <summary>
        /// Flag to denote if the bot is writing messages to its logs
        /// </summary>
        public bool IsLogging
        {
            get
            {
                var islogging = GlobalSettings.grabSetting("islogging");
                return islogging.ToLower() == "true";
            }
        }

        /// <summary>
        /// Flag to denote if the bot will email the botmaster using the AdminEmail setting should an error
        /// occur
        /// </summary>
        public bool WillCallHome
        {
            get
            {
                var willcallhome = GlobalSettings.grabSetting("willcallhome");
                return willcallhome.ToLower() == "true";
            }
        }

        /// <summary>
        /// When the Bot was initialised
        /// </summary>
        public DateTime StartedOn = DateTime.Now;

        /// <summary>
        /// The supposed sex of the bot
        /// </summary>
        public Gender Sex
        {
            get
            {
                var sex = Convert.ToInt32(GlobalSettings.grabSetting("gender"));
                Gender result;
                switch (sex)
                {
                    case -1:
                        result = Gender.Unknown;
                        break;
                    case 0:
                        result = Gender.Female;
                        break;
                    case 1:
                        result = Gender.Male;
                        break;
                    default:
                        result = Gender.Unknown;
                        break;
                }

                return result;
            }
        }

        /// <summary>
        /// The directory to look in for the AIML files
        /// </summary>
        public string PathToAiml => Path.Combine(_customPath, GlobalSettings.grabSetting("aimldirectory"));

        /// <summary>
        /// The directory to look in for the various XML configuration files
        /// </summary>
        public string PathToConfigFiles => Path.Combine(_customPath, GlobalSettings.grabSetting("configdirectory"));

        /// <summary>
        /// The directory into which the various log files will be written
        /// </summary>
        public string PathToLogs => Path.Combine(_customPath, GlobalSettings.grabSetting("logdirectory"));

        /// <summary>
        /// The number of categories this bot has in its graphmaster "brain"
        /// </summary>
        public int Size;

        /// <summary>
        /// The "brain" of the bot
        /// </summary>
        public Node Graphmaster;

        /// <summary>
        /// If set to false the input from AIML files will undergo the same normalization process that
        /// user input goes through. If true the bot will assume the AIML is correct. Defaults to true.
        /// </summary>
        public bool trustAiml = true;

        /// <summary>
        /// The maximum number of characters a "that" element of a path is allowed to be. Anything above
        /// this length will cause "that" to be "*". This is to avoid having the graphmaster process
        /// huge "that" elements in the path that might have been caused by the bot reporting third party
        /// data.
        /// </summary>
        public int MaxThatSize = 256;
        
        public string CustomResourcePath
        {
            get => _customPath;
            set => _customPath = value;
        }
        
        /// <summary>
        /// The last message to be entered into the log (for testing purposes)
        /// </summary>
        public string LastLogMessage = string.Empty;

        #endregion

        #region Events

        public event Action WrittenToLog;

        #endregion

        /// <summary>
        /// Ctor
        /// </summary>
        public Bot()
        {
            Setup();
        }

        #region Settings methods

        /// <summary>
        /// Loads AIML from .aiml files into the graphmaster "brain" of the bot
        /// </summary>
        public void LoadAimlFromFiles()
        {
            var loader = new AIMLLoader(this);
            loader.loadAIML();
        }

        /// <summary>
        /// Allows the bot to load a new XML version of some AIML
        /// </summary>
        /// <param name="newAiml">The XML document containing the AIML</param>
        /// <param name="filename">The originator of the XML document</param>
        public void LoadAimlFromXML(XmlDocument newAiml, string filename)
        {
            var loader = new AIMLLoader(this);
            loader.loadAIMLFromXML(newAiml, filename);
        }

        /// <summary>
        /// Instantiates the dictionary objects and collections associated with this class
        /// </summary>
        private void Setup()
        {
            GlobalSettings = new SettingsDictionary(this);
            GenderSubstitutions = new SettingsDictionary(this);
            Person2Substitutions = new SettingsDictionary(this);
            PersonSubstitutions = new SettingsDictionary(this);
            Substitutions = new SettingsDictionary(this);
            DefaultPredicates = new SettingsDictionary(this);
            _customTags = new Dictionary<string, TagHandler>();
            Graphmaster = new Node();
        }

        public void LoadSettings()
        {
            // try a safe default setting for the settings xml file
            var path = Path.Combine(_customPath, Path.Combine("config", "Settings.xml"));
            LoadSettings(path);
        }

        /// <summary>
        /// Loads settings and configuration info from various xml files referenced in the settings file passed in the args. 
        /// Also generates some default values if such values have not been set by the settings file.
        /// </summary>
        /// <param name="pathToSettings">Path to the settings xml file</param>
        public void LoadSettings(string pathToSettings)
        {
            GlobalSettings.loadSettings(pathToSettings);

            // Checks for some important default settings
            if (!GlobalSettings.containsSettingCalled("version"))
                GlobalSettings.addSetting("version", Environment.Version.ToString());
            if (!GlobalSettings.containsSettingCalled("name")) GlobalSettings.addSetting("name", "Unknown");
            if (!GlobalSettings.containsSettingCalled("botmaster")) GlobalSettings.addSetting("botmaster", "Unknown");
            if (!GlobalSettings.containsSettingCalled("master")) GlobalSettings.addSetting("botmaster", "Unknown");
            if (!GlobalSettings.containsSettingCalled("author"))
                GlobalSettings.addSetting("author", "Nicholas H.Tollervey");
            if (!GlobalSettings.containsSettingCalled("location")) GlobalSettings.addSetting("location", "Unknown");
            if (!GlobalSettings.containsSettingCalled("gender")) GlobalSettings.addSetting("gender", "-1");
            if (!GlobalSettings.containsSettingCalled("birthday")) GlobalSettings.addSetting("birthday", "2006/11/08");
            if (!GlobalSettings.containsSettingCalled("birthplace"))
                GlobalSettings.addSetting("birthplace", "Towcester, Northamptonshire, UK");
            if (!GlobalSettings.containsSettingCalled("website"))
                GlobalSettings.addSetting("website", "http://sourceforge.net/projects/aimlbot");
            if (GlobalSettings.containsSettingCalled("adminemail"))
            {
                var emailToCheck = GlobalSettings.grabSetting("adminemail");
                AdminEmail = emailToCheck;
            }
            else
            {
                GlobalSettings.addSetting("adminemail", "");
            }

            if (!GlobalSettings.containsSettingCalled("islogging")) GlobalSettings.addSetting("islogging", "False");
            if (!GlobalSettings.containsSettingCalled("willcallhome"))
                GlobalSettings.addSetting("willcallhome", "False");
            if (!GlobalSettings.containsSettingCalled("timeout")) GlobalSettings.addSetting("timeout", "2000");
            if (!GlobalSettings.containsSettingCalled("timeoutmessage"))
                GlobalSettings.addSetting("timeoutmessage", "ERROR: The request has timed out.");
            if (!GlobalSettings.containsSettingCalled("culture")) GlobalSettings.addSetting("culture", "en-US");
            if (!GlobalSettings.containsSettingCalled("splittersfile"))
                GlobalSettings.addSetting("splittersfile", "Splitters.xml");
            if (!GlobalSettings.containsSettingCalled("person2substitutionsfile"))
                GlobalSettings.addSetting("person2substitutionsfile", "Person2Substitutions.xml");
            if (!GlobalSettings.containsSettingCalled("personsubstitutionsfile"))
                GlobalSettings.addSetting("personsubstitutionsfile", "PersonSubstitutions.xml");
            if (!GlobalSettings.containsSettingCalled("gendersubstitutionsfile"))
                GlobalSettings.addSetting("gendersubstitutionsfile", "GenderSubstitutions.xml");
            if (!GlobalSettings.containsSettingCalled("defaultpredicates"))
                GlobalSettings.addSetting("defaultpredicates", "DefaultPredicates.xml");
            if (!GlobalSettings.containsSettingCalled("substitutionsfile"))
                GlobalSettings.addSetting("substitutionsfile", "Substitutions.xml");
            if (!GlobalSettings.containsSettingCalled("aimldirectory"))
                GlobalSettings.addSetting("aimldirectory", "aiml");
            if (!GlobalSettings.containsSettingCalled("configdirectory"))
                GlobalSettings.addSetting("configdirectory", "config");
            if (!GlobalSettings.containsSettingCalled("logdirectory"))
                GlobalSettings.addSetting("logdirectory", "logs");
            if (!GlobalSettings.containsSettingCalled("maxlogbuffersize"))
                GlobalSettings.addSetting("maxlogbuffersize", "64");
            if (!GlobalSettings.containsSettingCalled("notacceptinguserinputmessage"))
                GlobalSettings.addSetting("notacceptinguserinputmessage",
                    "This bot is currently set to not accept user input.");
            if (!GlobalSettings.containsSettingCalled("stripperregex"))
                GlobalSettings.addSetting("stripperregex", "[^0-9a-zA-Z]");

            // Load the dictionaries for this Bot from the various configuration files
            Person2Substitutions.loadSettings(Path.Combine(PathToConfigFiles,
                GlobalSettings.grabSetting("person2substitutionsfile")));
            PersonSubstitutions.loadSettings(Path.Combine(PathToConfigFiles,
                GlobalSettings.grabSetting("personsubstitutionsfile")));
            GenderSubstitutions.loadSettings(Path.Combine(PathToConfigFiles,
                GlobalSettings.grabSetting("gendersubstitutionsfile")));
            DefaultPredicates.loadSettings(Path.Combine(PathToConfigFiles,
                GlobalSettings.grabSetting("defaultpredicates")));
            Substitutions.loadSettings(Path.Combine(PathToConfigFiles,
                GlobalSettings.grabSetting("substitutionsfile")));

            // Grab the splitters for this bot
            LoadSplitters(Path.Combine(PathToConfigFiles, GlobalSettings.grabSetting("splittersfile")));
        }

        /// <summary>
        /// Loads the splitters for this bot from the supplied config file (or sets up some safe defaults)
        /// </summary>
        /// <param name="pathToSplitters">Path to the config file</param>
        private void LoadSplitters(string pathToSplitters)
        {
            var splittersFile = new FileInfo(pathToSplitters);
            if (splittersFile.Exists)
            {
                var splittersXmlDoc = new XmlDocument();
                splittersXmlDoc.Load(pathToSplitters);
                // the XML should have an XML declaration like this:
                // <?xml version="1.0" encoding="utf-8" ?> 
                // followed by a <root> tag with children of the form:
                // <item value="value"/>
                if (splittersXmlDoc.ChildNodes.Count == 2)
                {
                    if (splittersXmlDoc.LastChild.HasChildNodes)
                    {
                        foreach (XmlNode myNode in splittersXmlDoc.LastChild.ChildNodes)
                        {
                            if ((myNode.Name == "item") & (myNode.Attributes.Count == 1))
                            {
                                var value = myNode.Attributes["value"].Value;
                                Splitters.Add(value);
                            }
                        }
                    }
                }
            }

            if (Splitters.Count != 0) return;
            
            // we don't have any splitters, so lets make do with these...
            Splitters.Add(".");
            Splitters.Add("!");
            Splitters.Add("?");
            Splitters.Add(";");
        }

        public void LoadSplittersXml(XmlDocument splittersXmlDoc)
        {
            //  XmlDocument splittersXmlDoc = new XmlDocument();
            //  splittersXmlDoc.Load(pathToSplitters);

            // the XML should have an XML declaration like this:
            // <?xml version="1.0" encoding="utf-8" ?> 
            // followed by a <root> tag with children of the form:
            // <item value="value"/>
            if (splittersXmlDoc.ChildNodes.Count == 2)
            {
                if (splittersXmlDoc.LastChild.HasChildNodes)
                {
                    foreach (XmlNode myNode in splittersXmlDoc.LastChild.ChildNodes)
                    {
                        if (!((myNode.Name == "item") & (myNode.Attributes.Count == 1))) continue;
                        var value = myNode.Attributes["value"].Value;
                        Splitters.Add(value);
                    }
                }
            }

            if (Splitters.Count != 0) return;
            // we don't have any splitters, so lets make do with these...
            Splitters.Add(".");
            Splitters.Add("!");
            Splitters.Add("?");
            Splitters.Add(";");
        }

        #endregion

        #region Logging methods

        /// <summary>
        /// Writes a (timestamped) message to the bot's log.
        /// 
        /// Log files have the form of yyyyMMdd.log.
        /// </summary>
        /// <param name="message">The message to log</param>
        public void WriteToLog(string message)
        {
            LastLogMessage = message;
            if (IsLogging)
            {
                _logBuffer.Add(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + message + Environment.NewLine);
                if (_logBuffer.Count > MaxLogBufferSize - 1)
                {
                    // Write out to log file
                    var logDirectory = new DirectoryInfo(PathToLogs);
                    if (!logDirectory.Exists) logDirectory.Create();

                    var logFileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
                    var logFile = new FileInfo(Path.Combine(PathToLogs, logFileName));
                    StreamWriter writer;
                    if (!logFile.Exists)
                        writer = logFile.CreateText();
                    else
                        writer = logFile.AppendText();

                    foreach (var msg in _logBuffer) writer.WriteLine(msg);
                    writer.Close();
                    _logBuffer.Clear();
                }
            }

            if (!Equals(null, WrittenToLog)) WrittenToLog();
        }

        #endregion

        #region Conversation methods

        /// <summary>
        /// Given some raw input and a unique ID creates a response for a new user
        /// </summary>
        /// <param name="rawInput">the raw input</param>
        /// <param name="userGuid">an ID for the new user (referenced in the result object)</param>
        /// <returns>the result to be output to the user</returns>
        public Result Chat(string rawInput, string userGuid)
        {
            var request = new Request(rawInput, new User(userGuid, this), this);
            return Chat(request);
        }

        /// <summary>
        /// Given a request containing user input, produces a result from the bot
        /// </summary>
        /// <param name="request">the request from the user</param>
        /// <returns>the result to be output to the user</returns>
        public Result Chat(Request request)
        {
            var result = new Result(request.user, this, request);

            if (isAcceptingUserInput)
            {
                // Normalize the input
                var loader = new AIMLLoader(this);
                var splitter = new Normalize.SplitIntoSentences(this);
                var rawSentences = splitter.Transform(request.rawInput);
                foreach (var sentence in rawSentences)
                {
                    result.InputSentences.Add(sentence);
                    var path = loader.generatePath(sentence, request.user.getLastBotOutput(), request.user.Topic, true);
                    result.NormalizedPaths.Add(path);
                }

                // grab the templates for the various sentences from the graphmaster
                foreach (var path in result.NormalizedPaths)
                {
                    var query = new SubQuery(path);
                    query.Template =
                        Graphmaster.evaluate(path, query, request, MatchState.UserInput, new StringBuilder());
                    result.SubQueries.Add(query);
                }

                // process the templates into appropriate output
                foreach (var query in result.SubQueries)
                    if (query.Template.Length > 0)
                        try
                        {
                            var templateNode = AIMLTagHandler.getNode(query.Template);
                            var outputSentence = ProcessNode(templateNode, query, request, result, request.user);
                            if (outputSentence.Length > 0) result.OutputSentences.Add(outputSentence);
                        }
                        catch (Exception e)
                        {
                            if (WillCallHome) PhoneHome(e.Message, request);
                            WriteToLog("WARNING! A problem was encountered when trying to process the input: " +
                                       request.rawInput + " with the template: \"" + query.Template + "\"");
                        }
            }
            else
            {
                result.OutputSentences.Add(NotAcceptingUserInputMessage);
            }

            // populate the Result object
            result.Duration = DateTime.Now - request.StartedOn;
            request.user.addResult(result);

            return result;
        }

        /// <summary>
        /// Recursively evaluates the template nodes returned from the bot
        /// </summary>
        /// <param name="node">the node to evaluate</param>
        /// <param name="query">the query that produced this node</param>
        /// <param name="request">the request from the user</param>
        /// <param name="result">the result to be sent to the user</param>
        /// <param name="user">the user who originated the request</param>
        /// <returns>the output string</returns>
        private string ProcessNode(XmlNode node, SubQuery query, Request request, Result result, User user)
        {
            // check for timeout (to avoid infinite loops)
            if (request.StartedOn.AddMilliseconds(request.bot.TimeOut) < DateTime.Now)
            {
                request.bot.WriteToLog("WARNING! Request timeout. User: " + request.user.UserID + " raw input: \"" +
                                       request.rawInput + "\" processing template: \"" + query.Template + "\"");
                request.hasTimedOut = true;
                return string.Empty;
            }

            // process the node
            var tagName = node.Name.ToLower();
            if (tagName == "template")
            {
                var templateResult = new StringBuilder();
                if (node.HasChildNodes)
                {
                    // recursively check
                    foreach (XmlNode childNode in node.ChildNodes)
                        templateResult.Append(ProcessNode(childNode, query, request, result, user));
                }

                return templateResult.ToString();
            }

            AIMLTagHandler tagHandler = null;
            tagHandler = GetBespokeTags(user, query, request, result, node);
            if (Equals(null, tagHandler))
                switch (tagName)
                {
                    case "bot":
                        tagHandler = new AIMLTagHandlers.bot(this, user, query, request, result, node);
                        break;
                    case "condition":
                        tagHandler = new AIMLTagHandlers.condition(this, user, query, request, result, node);
                        break;
                    case "date":
                        tagHandler = new AIMLTagHandlers.date(this, user, query, request, result, node);
                        break;
                    case "formal":
                        tagHandler = new AIMLTagHandlers.formal(this, user, query, request, result, node);
                        break;
                    case "gender":
                        tagHandler = new AIMLTagHandlers.gender(this, user, query, request, result, node);
                        break;
                    case "get":
                        tagHandler = new AIMLTagHandlers.get(this, user, query, request, result, node);
                        break;
                    case "gossip":
                        tagHandler = new AIMLTagHandlers.gossip(this, user, query, request, result, node);
                        break;
                    case "id":
                        tagHandler = new AIMLTagHandlers.id(this, user, query, request, result, node);
                        break;
                    case "input":
                        tagHandler = new AIMLTagHandlers.input(this, user, query, request, result, node);
                        break;
                    case "javascript":
                        tagHandler = new AIMLTagHandlers.javascript(this, user, query, request, result, node);
                        break;
                    case "learn":
                        tagHandler = new AIMLTagHandlers.learn(this, user, query, request, result, node);
                        break;
                    case "lowercase":
                        tagHandler = new AIMLTagHandlers.lowercase(this, user, query, request, result, node);
                        break;
                    case "person":
                        tagHandler = new AIMLTagHandlers.person(this, user, query, request, result, node);
                        break;
                    case "person2":
                        tagHandler = new AIMLTagHandlers.person2(this, user, query, request, result, node);
                        break;
                    case "random":
                        tagHandler = new AIMLTagHandlers.random(this, user, query, request, result, node);
                        break;
                    case "sentence":
                        tagHandler = new AIMLTagHandlers.sentence(this, user, query, request, result, node);
                        break;
                    case "set":
                        tagHandler = new AIMLTagHandlers.set(this, user, query, request, result, node);
                        break;
                    case "size":
                        tagHandler = new AIMLTagHandlers.size(this, user, query, request, result, node);
                        break;
                    case "sr":
                        tagHandler = new AIMLTagHandlers.sr(this, user, query, request, result, node);
                        break;
                    case "srai":
                        tagHandler = new AIMLTagHandlers.srai(this, user, query, request, result, node);
                        break;
                    case "star":
                        tagHandler = new AIMLTagHandlers.star(this, user, query, request, result, node);
                        break;
                    case "system":
                        tagHandler = new AIMLTagHandlers.system(this, user, query, request, result, node);
                        break;
                    case "that":
                        tagHandler = new AIMLTagHandlers.that(this, user, query, request, result, node);
                        break;
                    case "thatstar":
                        tagHandler = new AIMLTagHandlers.thatstar(this, user, query, request, result, node);
                        break;
                    case "think":
                        tagHandler = new AIMLTagHandlers.think(this, user, query, request, result, node);
                        break;
                    case "topicstar":
                        tagHandler = new AIMLTagHandlers.topicstar(this, user, query, request, result, node);
                        break;
                    case "uppercase":
                        tagHandler = new AIMLTagHandlers.uppercase(this, user, query, request, result, node);
                        break;
                    case "version":
                        tagHandler = new AIMLTagHandlers.version(this, user, query, request, result, node);
                        break;
                    default:
                        tagHandler = null;
                        break;
                }

            if (Equals(null, tagHandler))
            {
                return node.InnerText;
            }

            if (tagHandler.isRecursive)
            {
                if (node.HasChildNodes)
                    // recursively check
                    foreach (XmlNode childNode in node.ChildNodes)
                        if (childNode.NodeType != XmlNodeType.Text)
                            childNode.InnerXml = ProcessNode(childNode, query, request, result, user);
                return tagHandler.Transform();
            }

            var resultNodeInnerXML = tagHandler.Transform();
            var resultNode = AIMLTagHandler.getNode("<node>" + resultNodeInnerXML + "</node>");
            if (resultNode.HasChildNodes)
            {
                var recursiveResult = new StringBuilder();
                // recursively check
                foreach (XmlNode childNode in resultNode.ChildNodes)
                    recursiveResult.Append(ProcessNode(childNode, query, request, result, user));
                return recursiveResult.ToString();
            }

            return resultNode.InnerXml;
        }

        /// <summary>
        /// Searches the CustomTag collection and processes the AIML if an appropriate tag handler is found
        /// </summary>
        /// <param name="user">the user who originated the request</param>
        /// <param name="query">the query that produced this node</param>
        /// <param name="request">the request from the user</param>
        /// <param name="result">the result to be sent to the user</param>
        /// <param name="node">the node to evaluate</param>
        /// <returns>the output string</returns>
        public AIMLTagHandler GetBespokeTags(User user, SubQuery query, Request request, Result result, XmlNode node)
        {
            if (_customTags.ContainsKey(node.Name.ToLower()))
            {
                var customTagHandler = _customTags[node.Name.ToLower()];

                var newCustomTag = customTagHandler.Instantiate(_lateBindingAssemblies);
                if (Equals(null, newCustomTag))
                {
                    return null;
                }

                newCustomTag.user = user;
                newCustomTag.query = query;
                newCustomTag.request = request;
                newCustomTag.result = result;
                newCustomTag.templateNode = node;
                newCustomTag.bot = this;
                return newCustomTag;
            }

            return null;
        }

        #endregion

        #region Serialization

        /// <summary>
        /// Saves the graphmaster node (and children) to a binary file to avoid processing the AIML each time the 
        /// bot starts
        /// </summary>
        /// <param name="path">the path to the file for saving</param>
        public void SaveToBinaryFile(string path)
        {
            // check to delete an existing version of the file
            var fi = new FileInfo(path);
            if (fi.Exists)
            {
#if !UNITY_WEBPLAYER && !UNITY_WEBGL
                fi.Delete();
#endif
            }

            var saveFile = File.Create(path);
            var bf = new BinaryFormatter();
            bf.Serialize(saveFile, Graphmaster);
            saveFile.Close();
        }

        /// <summary>
        /// Loads a dump of the graphmaster into memory so avoiding processing the AIML files again
        /// </summary>
        /// <param name="path">the path to the dump file</param>
        public void LoadFromBinaryFile(string path)
        {
            var loadFile = File.OpenRead(path);
            var bf = new BinaryFormatter();
            Graphmaster = (Node) bf.Deserialize(loadFile);
            loadFile.Close();
        }

        #endregion

        #region Latebinding custom-tag dll handlers

        /// <summary>
        /// Loads any custom tag handlers found in the dll referenced in the argument
        /// </summary>
        /// <param name="pathToDLL">the path to the dll containing the custom tag handling code</param>
        public void LoadCustomTagHandlers(string pathToDLL)
        {
            var tagDLL = Assembly.LoadFrom(pathToDLL);
            var tagDLLTypes = tagDLL.GetTypes();
            for (var i = 0; i < tagDLLTypes.Length; i++)
            {
                var typeCustomAttributes = tagDLLTypes[i].GetCustomAttributes(false);
                for (var j = 0; j < typeCustomAttributes.Length; j++)
                {
                    if (typeCustomAttributes[j] is not CustomTagAttribute) continue;
                    
                    // We've found a custom tag handling class
                    // so store the assembly and store it away in the Dictionary<,> as a TagHandler class for 
                    // later usage
                    // store Assembly
                    if (!_lateBindingAssemblies.ContainsKey(tagDLL.FullName))
                        _lateBindingAssemblies.Add(tagDLL.FullName, tagDLL);

                    // create the TagHandler representation
                    var newTagHandler = new TagHandler();
                    newTagHandler.AssemblyName = tagDLL.FullName;
                    newTagHandler.ClassName = tagDLLTypes[i].FullName;
                    newTagHandler.TagName = tagDLLTypes[i].Name.ToLower();
                    if (_customTags.ContainsKey(newTagHandler.TagName))
                        throw new Exception("ERROR! Unable to add the custom tag: <" + newTagHandler.TagName +
                                            ">, found in: " + pathToDLL +
                                            " as a handler for this tag already exists.");
                    _customTags.Add(newTagHandler.TagName, newTagHandler);
                }
            }
        }

        #endregion

        #region Phone Home

        /// <summary>
        /// Attempts to send an email to the botmaster at the AdminEmail address setting with error messages
        /// resulting from a query to the bot
        /// </summary>
        /// <param name="errorMessage">the resulting error message</param>
        /// <param name="request">the request object that encapsulates all sorts of useful information</param>
        public void PhoneHome(string errorMessage, Request request)
        {
        }

        #endregion
    }
}