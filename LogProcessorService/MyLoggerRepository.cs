using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LogProcessorService
{
    public static class Log
    {
        #region property
        public static MyLoggerWrapper Root
        {
            get
            {
                if (null == s_rootLogger)
                {
                    s_rootLogger = MyLoggerWrapper.Create("Root");
                }

                return s_rootLogger;
            }
        }

        public static MyLoggerWrapper Kernel
        {
            get
            {
                if (null == s_kernelLogger)
                {
                    s_kernelLogger = MyLoggerWrapper.Create("Kernel");
                }

                return s_kernelLogger;
            }
        }

        public static MyLoggerWrapper Action
        {
            get
            {
                if (null == s_actionLogger)
                {
                    s_actionLogger = MyLoggerWrapper.Create("Action");
                }

                return s_actionLogger;
            }
        }

        public static MyLoggerWrapper MaintenanceAction
        {
            get
            {
                if (null == s_maintenanceActionLogger)
                {
                    s_maintenanceActionLogger = MyLoggerWrapper.Create("MaintenanceAction");
                }

                return s_maintenanceActionLogger;
            }
        }

        public static MyLoggerWrapper Workflow
        {
            get
            {
                if (null == s_workflowLogger)
                {
                    s_workflowLogger = MyLoggerWrapper.Create("Workflow");
                }

                return s_workflowLogger;
            }
        }

        public static MyLoggerWrapper BusinessService
        {
            get
            {
                if (null == s_businessServiceLogger)
                {
                    s_businessServiceLogger = MyLoggerWrapper.Create("BusinessService");
                }

                return s_businessServiceLogger;
            }
        }

        public static MyLoggerWrapper UIService
        {
            get
            {
                if (null == s_UILogger)
                {
                    s_UILogger = MyLoggerWrapper.Create("UIService");
                }

                return s_UILogger;
            }
        }

        public static MyLoggerWrapper Engine
        {
            get
            {
                if (null == s_engineLogger)
                {
                    s_engineLogger = MyLoggerWrapper.Create("Engine");
                }

                return s_engineLogger;
            }
        }

        public static MyLoggerWrapper Device
        {
            get
            {
                if (null == s_deviceLogger)
                {
                    s_deviceLogger = MyLoggerWrapper.Create("Device");
                }

                return s_deviceLogger;
            }
        }

        public static MyLoggerWrapper NetworkLogger
        {
            get
            {
                if (null == s_networkLogger)
                {
                    s_networkLogger = MyLoggerWrapper.Create("Network");
                }

                return s_networkLogger;
            }
        }

        public static MyLoggerWrapper ISO8583
        {
            get
            {
                if (null == s_iso8583Logger)
                {
                    s_iso8583Logger = MyLoggerWrapper.Create("ISO8583");
                }

                return s_iso8583Logger;
            }
        }

        public static MyLoggerWrapper HouseKeeper
        {
            get
            {
                if (null == s_houseKeeperLogger)
                {
                    s_houseKeeperLogger = MyLoggerWrapper.Create("HouseKeeper");
                }

                return s_houseKeeperLogger;
            }
        }

        public static MyLoggerWrapper ElectricJournal
        {
            get
            {
                if (null == s_electricJournalLogger)
                {
                    s_electricJournalLogger = MyLoggerWrapper.Create("ElectricJournal");
                }

                return s_electricJournalLogger;
            }
        }

        public static MyLoggerWrapper XDCScreenParser
        {
            get
            {
                if (null == s_xdcScreenParserLog)
                {
                    s_xdcScreenParserLog = MyLoggerWrapper.Create("XDCScreenParser");
                }

                return s_xdcScreenParserLog;
            }
        }

        /// <summary>
        /// The definition of the XDC Electronic Log object
        /// </summary>
        public static MyLoggerWrapper XdcEJ
        {
            get
            {
                if (null == s_xdcEJLogger)
                {
                    s_xdcEJLogger = MyLoggerWrapper.Create("XdcTrace.XdcEJ");
                }

                return s_xdcEJLogger;
            }
        }

        /// <summary>
        /// The definition of the XDC Trace Log object
        /// </summary>
        public static MyLoggerWrapper XdcTrace
        {
            get
            {
                if (null == s_xdcTraceLogger)
                {
                    s_xdcTraceLogger = MyLoggerWrapper.Create("XdcTrace");
                }

                return s_xdcTraceLogger;
            }
        }
        /// <summary>
        /// The definition of the XDC Device Status log object
        /// </summary>
        public static MyLoggerWrapper XdcDeviceStatus
        {
            get
            {
                if (null == s_xdcDeviceStatusLogger)
                {
                    s_xdcDeviceStatusLogger = MyLoggerWrapper.Create("XdcDeviceStatus");
                }

                return s_xdcDeviceStatusLogger;
            }
        }
        public static MyLoggerWrapper DataGateway
        {
            get
            {
                if (null == s_dataGatewayLogger)
                {
                    s_dataGatewayLogger = MyLoggerWrapper.Create("BusinessService.DataGateway");
                }

                return s_dataGatewayLogger;
            }
        }

        public static MyLoggerWrapper ResourceManager
        {
            get
            {
                if (null == s_resourceManager)
                {
                    s_resourceManager = MyLoggerWrapper.Create("ResourceManager");
                }
                return s_resourceManager;
            }
        }

        public static MyLoggerWrapper COMLog
        {
            get
            {
                if (null == s_comLog)
                {
                    s_comLog = MyLoggerWrapper.Create("COMLog");
                }

                return s_comLog;
            }
        }

        public static MyLoggerWrapper FileTransfer
        {
            get
            {
                if (null == s_fileTransfer)
                {
                    s_fileTransfer = MyLoggerWrapper.Create("FileTransfer");
                }

                return s_fileTransfer;
            }
        }

        public static MyLoggerWrapper WCFServiceHost
        {
            get
            {
                if (null == s_wcfServiceHost)
                {
                    s_wcfServiceHost = MyLoggerWrapper.Create("wcfServiceHost");
                }

                return s_wcfServiceHost;
            }
        }

        public static MyLoggerWrapper Project
        {
            get
            {
                if (null == s_projectLogger)
                {
                    s_projectLogger = MyLoggerWrapper.Create("Project");
                }

                return s_projectLogger;
            }
        }

        public static MyLoggerWrapper DataCache
        {
            get
            {
                if (null == s_dataCacheLogger)
                {
                    s_dataCacheLogger = MyLoggerWrapper.Create("DataCache");
                }

                return s_dataCacheLogger;
            }
        }

        public static MyLoggerWrapper Validation
        {
            get
            {
                if (null == s_validationLogger)
                {
                    s_validationLogger = MyLoggerWrapper.Create("Validation");
                }

                return s_validationLogger;
            }
        }

        public static MyLoggerWrapper Keeper
        {
            get
            {
                if (null == s_keeperLogger)
                {
                    s_keeperLogger = MyLoggerWrapper.Create("Keeper");
                }

                return s_keeperLogger;
            }
        }
        #endregion

        #region field
        private static MyLoggerWrapper s_rootLogger = null;

        private static MyLoggerWrapper s_kernelLogger = null;

        private static MyLoggerWrapper s_actionLogger = null;

        private static MyLoggerWrapper s_maintenanceActionLogger = null;

        private static MyLoggerWrapper s_businessServiceLogger = null;

        private static MyLoggerWrapper s_engineLogger = null;

        private static MyLoggerWrapper s_workflowLogger = null;

        private static MyLoggerWrapper s_deviceLogger = null;

        private static MyLoggerWrapper s_networkLogger = null;

        private static MyLoggerWrapper s_UILogger = null;

        private static MyLoggerWrapper s_iso8583Logger = null;

        private static MyLoggerWrapper s_houseKeeperLogger = null;

        private static MyLoggerWrapper s_electricJournalLogger = null;

        private static MyLoggerWrapper s_xdcScreenParserLog = null;

        private static MyLoggerWrapper s_xdcEJLogger = null;

        private static MyLoggerWrapper s_xdcTraceLogger = null;

        private static MyLoggerWrapper s_xdcDeviceStatusLogger = null;

        private static MyLoggerWrapper s_dataGatewayLogger = null;

        private static MyLoggerWrapper s_resourceManager = null;

        private static MyLoggerWrapper s_comLog = null;

        private static MyLoggerWrapper s_fileTransfer = null;

        private static MyLoggerWrapper s_wcfServiceHost = null;

        private static MyLoggerWrapper s_projectLogger = null;

        private static MyLoggerWrapper s_dataCacheLogger = null;

        private static MyLoggerWrapper s_validationLogger = null;

        private static MyLoggerWrapper s_keeperLogger = null;
        #endregion
    }

    public struct AutoLoggerGuard : IDisposable
    {
        #region constructor
        public AutoLoggerGuard(MyLoggerWrapper argLogger,
                                string argScope)
        {
            Debug.Assert(null != argLogger);

            m_logger = argLogger;
            m_scope = argScope;

            m_logger.LogDebugFormat("Enter {0}", m_scope);
        }
        #endregion

        #region method
        void IDisposable.Dispose()
        {
            Debug.Assert(null != m_logger);

            m_logger.LogDebugFormat("Leave {0}", m_scope);
        }
        #endregion

        #region field
        private string m_scope;

        private MyLoggerWrapper m_logger;
        #endregion
    }
}
