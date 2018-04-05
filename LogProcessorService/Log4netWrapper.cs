using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.IO;
using System.Diagnostics;
using System.Xml;
using HelperService;
using System.Xml.Linq;

namespace LogProcessorService
{
    public class Log4netWrapper : IMyLogger
    {
        #region constructor
        private Log4netWrapper(ILog iLog, string argName)
        {
            Debug.Assert(null != iLog && Opened && !string.IsNullOrEmpty(argName));

            m_iLog = iLog;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"config\logconfig.xml");
                XmlNode refNode = doc.DocumentElement.SelectSingleNode(string.Format(s_appendRefFormat, argName));
                if (null != refNode)
                {
                    XmlAttribute refAttri = refNode.Attributes[s_refAttri];
                    if (null != refAttri &&
                         !string.IsNullOrEmpty(refAttri.Value))
                    {
                        XmlNodeList listFilterNodes = doc.DocumentElement.SelectNodes(string.Format(s_filterFormat, refAttri.Value));
                        if (0 != listFilterNodes.Count)
                        {
                            XmlAttribute valueAttri = null;
                            XmlAttribute enableAttri = null;
                            int temp = 0;
                            foreach (XmlNode filterNode in listFilterNodes)
                            {
                                valueAttri = filterNode.Attributes[s_valueAttri];
                                enableAttri = filterNode.Attributes[s_enableAttri];
                                if (null == valueAttri ||
                                     string.IsNullOrEmpty(valueAttri.Value) ||
                                     null == enableAttri ||
                                     string.IsNullOrEmpty(enableAttri.Value))
                                {
                                    continue;
                                }

                                if (!int.TryParse(enableAttri.Value, out temp))
                                {
                                    continue;
                                }

                                try
                                {
                                    Filters.Add(valueAttri.Value, 0 == temp ? false : true);
                                }
                                catch (System.Exception ex)
                                {
                                    Trace.TraceWarning("The filter [{0}] is duplicate", valueAttri.Value);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                return;
            }
        }

        static Log4netWrapper()
        {
            try
            {
                string cfgFile = AppDomain.CurrentDomain.BaseDirectory + @"config\logconfig.xml";
                FileInfo cfgInfo = new FileInfo(cfgFile);

                SolidXmlDocument doc = new SolidXmlDocument();
                doc.Load(AppDomain.CurrentDomain.BaseDirectory + @"config\logconfig.xml");
                XmlAttribute attri = doc.DocumentElement.Attributes["showHex"];
                if (null != attri &&
                     !string.IsNullOrEmpty(attri.Value))
                {
                    int value = 0;
                    if (int.TryParse(attri.Value, out value))
                    {
                        s_showHex = value == 0 ? false : true;
                    }
                }
                XmlNode encryptNode = doc.DocumentElement.SelectSingleNode(s_encryptNode);
                if (null != encryptNode)
                {
                    XmlAttribute valueAttri = encryptNode.Attributes[s_valueAttri];
                    if (null != valueAttri &&
                         !string.IsNullOrEmpty(valueAttri.Value))
                    {
                        LoggerKeyHelper.EncryptLoggerAppenders = valueAttri.Value.Split(
                            new[] { '|' },
                            StringSplitOptions.RemoveEmptyEntries
                        );
                    }
                }
                //string generalConfig = AppDomain.CurrentDomain.BaseDirectory + @"config\GeneralConfig.xml";

                //var root = XElement.Load(generalConfig);
                //var xElement = root.Element("EncryptLog");
                //if (xElement != null)
                //{
                //    var xAttribute = xElement.Attribute("value");
                //    if (xAttribute != null && !string.IsNullOrEmpty(xAttribute.Value))
                //    {
                //        LoggerKeyHelper.EncryptLoggerAppenders=xAttribute.Value.Split(
                //            new[] { '|' },
                //            StringSplitOptions.RemoveEmptyEntries
                //        );
                //    }
                //}

                XmlConfigurator.Configure(cfgInfo);
                m_bIsOpen = true;

                doc.RemoveAll();
                doc = null;
            }
            catch (System.Exception ex)
            {
                Trace.TraceError("Failed to startup the log4net service with error {0}", ex.Message);
                return;
            }
        }
        #endregion

        #region mehtods of the IGrgLogger interface
        void IMyLogger.Log(string message,
                             emLogLevel level)
        {
            lock (m_synLocker)
            {
                switch (level)
                {
                    case emLogLevel.Debug:
                        {
                            LogDebug(message);
                        }
                        break;

                    case emLogLevel.Info:
                        {
                            LogInfo(message);
                        }
                        break;

                    case emLogLevel.Warn:
                        {
                            LogWarn(message);
                        }
                        break;

                    case emLogLevel.Error:
                        {
                            LogError(message);
                        }
                        break;

                    case emLogLevel.Fatal:
                        {
                            LogFatal(message);
                        }
                        break;
                }
            }
        }

        void IMyLogger.LogHex(byte[] arrBuffer,
                               int argSize)
        {
            lock (m_synLocker)
            {
                LogInfo(s_showHex ? ConvertHelper.HexToString(arrBuffer, argSize) : Encoding.ASCII.GetString(arrBuffer, 0, argSize));
            }
        }

        void IMyLogger.LogHexFormat(string argFormat,
                                      byte[] arrBuffer,
                                      int argSize)
        {
            lock (m_synLocker)
            {
                LogInfoFormat(argFormat, s_showHex ? ConvertHelper.HexToString(arrBuffer, argSize) : Encoding.ASCII.GetString(arrBuffer, 0, argSize));
            }
        }

        void IMyLogger.Log(string message,
                             emLogLevel level,
                             Exception exp)
        {
            lock (m_synLocker)
            {
                switch (level)
                {
                    case emLogLevel.Debug:
                        {
                            LogDebug(message, exp);
                        }
                        break;

                    case emLogLevel.Info:
                        {
                            LogInfo(message, exp);
                        }
                        break;

                    case emLogLevel.Warn:
                        {
                            LogWarn(message, exp);
                        }
                        break;

                    case emLogLevel.Error:
                        {
                            LogError(message, exp);
                        }
                        break;

                    case emLogLevel.Fatal:
                        {
                            LogFatal(message, exp);
                        }
                        break;
                }
            }
        }

        void IMyLogger.Log(string message,
                             string key,
                             emLogLevel level)
        {
            if (null != m_dicFilter)
            {
                bool enable = false;
                if (m_dicFilter.TryGetValue(key, out enable))
                {
                    if (!enable)
                    {
                        return;
                    }
                }
            }

            lock (m_synLocker)
            {
                switch (level)
                {
                    case emLogLevel.Debug:
                        {
                            LogDebug(message);
                        }
                        break;

                    case emLogLevel.Info:
                        {
                            LogInfo(message);
                        }
                        break;

                    case emLogLevel.Warn:
                        {
                            LogWarn(message);
                        }
                        break;

                    case emLogLevel.Error:
                        {
                            LogError(message);
                        }
                        break;

                    case emLogLevel.Fatal:
                        {
                            LogFatal(message);
                        }
                        break;
                }
            }
        }

        void IMyLogger.Log(string message,
                             string key,
                             emLogLevel level,
                             Exception exp)
        {
            if (null != m_dicFilter)
            {
                bool enable = false;
                if (m_dicFilter.TryGetValue(key, out enable))
                {
                    if (!enable)
                    {
                        return;
                    }
                }
            }

            lock (m_synLocker)
            {
                switch (level)
                {
                    case emLogLevel.Debug:
                        {
                            LogDebug(message, exp);
                        }
                        break;

                    case emLogLevel.Info:
                        {
                            LogInfo(message, exp);
                        }
                        break;

                    case emLogLevel.Warn:
                        {
                            LogWarn(message, exp);
                        }
                        break;

                    case emLogLevel.Error:
                        {
                            LogError(message, exp);
                        }
                        break;

                    case emLogLevel.Fatal:
                        {
                            LogFatal(message, exp);
                        }
                        break;
                }
            }
        }

        void IMyLogger.LogFormat(string format,
                                   emLogLevel level,
                                   params object[] args)
        {
            lock (m_synLocker)
            {
                switch (level)
                {
                    case emLogLevel.Debug:
                        {
                            LogDebugFormat(format, args);
                        }
                        break;

                    case emLogLevel.Info:
                        {
                            LogInfoFormat(format, args);
                        }
                        break;

                    case emLogLevel.Warn:
                        {
                            LogWarnFormat(format, args);
                        }
                        break;

                    case emLogLevel.Error:
                        {
                            LogErrorFormat(format, args);
                        }
                        break;

                    case emLogLevel.Fatal:
                        {
                            LogFataFormat(format, args);
                        }
                        break;
                }
            }
        }

        void IMyLogger.LogFormat(string format,
                                   string key,
                                   emLogLevel level,
                                   params object[] args)
        {
            if (null != m_dicFilter)
            {
                bool enable = false;
                if (m_dicFilter.TryGetValue(key, out enable))
                {
                    if (!enable)
                    {
                        return;
                    }
                }
            }

            lock (m_synLocker)
            {
                switch (level)
                {
                    case emLogLevel.Debug:
                        {
                            LogDebugFormat(format, args);
                        }
                        break;

                    case emLogLevel.Info:
                        {
                            LogInfoFormat(format, args);
                        }
                        break;

                    case emLogLevel.Warn:
                        {
                            LogWarnFormat(format, args);
                        }
                        break;

                    case emLogLevel.Error:
                        {
                            LogErrorFormat(format, args);
                        }
                        break;

                    case emLogLevel.Fatal:
                        {
                            LogFataFormat(format, args);
                        }
                        break;
                }
            }
        }
        #endregion


        #region method
        //public static Log4netWrapper Create( string strName )
        //{
        //    Debug.Assert(!string.IsNullOrEmpty(strName) && Opened);

        //    return new Log4netWrapper( LogManager.GetLogger(strName) );
        //}
        public static IMyLogger Create(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name) && Opened);

            return new Log4netWrapper(LogManager.GetLogger(name), name) as IMyLogger;
        }

        public void LogDebug(string strMsg, Exception objExp = null)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsDebugEnabled)
            {
                if (null == objExp)
                {
                    m_iLog.Debug(strMsg);
                }
                else
                {
                    m_iLog.Debug(strMsg, objExp);
                }
            }
        }

        public void LogDebugFormat(string strFormat, params object[] arrObjs)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsDebugEnabled)
            {
                m_iLog.DebugFormat(strFormat, arrObjs);
            }
        }

        public void LogInfo(string strMsg, Exception objExp = null)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsInfoEnabled)
            {
                if (null == objExp)
                {
                    m_iLog.Info(strMsg);
                }
                else
                {
                    m_iLog.Info(strMsg, objExp);
                }
            }
        }

        public void LogInfo(string strMsg, Exception objExp, bool argDate)
        {
            Debug.Assert(null != m_iLog);

            if (argDate)
            {
                DateTime date = DateTime.Now;
                string strDateTime = string.Format(" {0:dd/MM/yyyy HH:mm:ss} ", date);
                strDateTime = strDateTime.Replace("-", "/");
                strMsg = strDateTime + strMsg;
            }

            if (m_iLog.IsInfoEnabled)
            {
                if (null == objExp)
                {
                    m_iLog.Info(strMsg);
                }
                else
                {
                    m_iLog.Info(strMsg, objExp);
                }
            }
        }

        public void LogInfoFormat(string strFormat, params object[] arrObjs)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsInfoEnabled)
            {
                m_iLog.InfoFormat(strFormat, arrObjs);
            }
        }

        public void LogWarn(string strMsg, Exception objExp = null)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsWarnEnabled)
            {
                if (null == objExp)
                {
                    m_iLog.Warn(strMsg);
                }
                else
                {
                    m_iLog.Warn(strMsg, objExp);
                }
            }
        }

        public void LogWarnFormat(string strFormat, params object[] arrObjs)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsWarnEnabled)
            {
                m_iLog.WarnFormat(strFormat, arrObjs);
            }
        }

        public void LogError(string strMsg, Exception objExp = null)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsErrorEnabled)
            {
                if (null == objExp)
                {
                    m_iLog.Error(strMsg);
                }
                else
                {
                    m_iLog.Error(strMsg, objExp);
                }
            }
        }

        public void LogErrorFormat(string strFormat, params object[] arrObjs)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsErrorEnabled)
            {
                m_iLog.ErrorFormat(strFormat, arrObjs);
            }
        }

        public void LogFatal(string strMsg, Exception objExp = null)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsFatalEnabled)
            {
                if (null == objExp)
                {
                    m_iLog.Fatal(strMsg);
                }
                else
                {
                    m_iLog.Fatal(strMsg, objExp);
                }
            }
        }

        public void LogFataFormat(string strFormat, params object[] arrObjs)
        {
            Debug.Assert(null != m_iLog);

            if (m_iLog.IsFatalEnabled)
            {
                m_iLog.FatalFormat(strFormat, arrObjs);
            }
        }
        #endregion

        #region property
        public static bool Opened
        {
            get
            {
                return m_bIsOpen;
            }
        }

        public Dictionary<string, bool> Filters
        {
            get
            {
                if (null == m_dicFilter)
                {
                    m_dicFilter = new Dictionary<string, bool>();
                }

                return m_dicFilter;
            }
        }
        #endregion

        #region field
        private static bool m_bIsOpen = false;

        private ILog m_iLog = null;

        private static bool s_showHex = true;

        private object m_synLocker = new object();

        private Dictionary<string, bool> m_dicFilter = null;

        private const string s_appendRefFormat = "logger[@name='{0}']/appender-ref";

        private const string s_filterFormat = "appender[@name='{0}']/GrgFilter/Keyword";

        private const string s_refAttri = "ref";

        private const string s_valueAttri = "value";

        private const string s_enableAttri = "enable";

        private const string s_encryptNode = "EncryptLog";

        private const string s_ElectricJournalAppenderAttri = "ElectricJournalAppender";
        #endregion
    }
}
