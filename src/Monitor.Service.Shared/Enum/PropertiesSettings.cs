﻿namespace Count4U.Model
{
	public static class PropertiesSettings
	{
		public const string AuditDBConnectionString = @"metadata=res://*/App_Data.AuditDB.csdl|res://*/App_Data.AuditDB.ssdl|res://*/App_Data.AuditDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source={0}""";
		public const string Count4UDBConnectionString = @"metadata=res://*/App_Data.Count4UDB.csdl|res://*/App_Data.Count4UDB.ssdl|res://*/App_Data.Count4UDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source={0};Default Lock Timeout=60000;  Max Database Size = {1}; Max Buffer Size = {2}""";
		public const string MainDBConnectionString = @"metadata=res://*/App_Data.MainDB.csdl|res://*/App_Data.MainDB.ssdl|res://*/App_Data.MainDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source={0}""";
		//public const string MainDBConnectionString = @"metadata=res://*/App_Data.MainDB.csdl|res://*/App_Data.MainDB.ssdl|res://*/App_Data.MainDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=&quot;Data Source={0}&quot;" ;
		public const string ProcessDBConnectionString = @"metadata=res://*/App_Data.ProcessDB.csdl|res://*/App_Data.ProcessDB.ssdl|res://*/App_Data.ProcessDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source={0}""";
		public const string AnalyticDBConnectionString = @"metadata=res://*/App_Data.AnalyticDB.csdl|res://*/App_Data.AnalyticDB.ssdl|res://*/App_Data.AnalyticDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source={0}""";
		public const string MonitorDBConnectionString = @"metadata=res://*/App_Data.MonitorDB.csdl|res://*/App_Data.MonitorDB.ssdl|res://*/App_Data.MonitorDB.msl;provider=System.Data.SqlServerCe.4.0;provider connection string=""Data Source={0}""";


		public const string FolderApp_Data = @"App_Data";
		public const string Count4UDBFile = @"Count4UDB.sdf";
		public const string EmptyCount4UDBFile = @"EmptyCount4UDB.sdf";
		public const string EmptyCount4UDBPath = @"C:\Temp\testDB\App_Data\EmptyCount4UDB.sdf";
		public const string DebugDbPath = @"\..\..\..\Count4U.Model\";
		public const string FolderInventor = @"Inventor";
		public const string FolderImport = @"ImportData";
		public const string DebugImportPath = @"\..\..\..\Count4U.Model\";
		public const string FolderCustomer = @"Customer";
		public const string FolderBranch = @"Branch";
		public const string FolderLogoFile = @"LogoFile";
		public const string AuditDBFile = @"AuditDB.sdf";
		public const string MainDBFile = @"MainDB.sdf";
		public const string SetupDbFolder = @"SetupDb";
		public const string ReportTemplateFolder = @"ReportTemplate";
		public const string ReportModulePath = @"\..\..\..\Count4U.GenerationReport\";
		public const string ExportToPDAFolder = @"ExportToPDA";
		public const string DebugExportToPDAPath = @"\..\..\..\Count4U.Model\";
		public const string UploadFolder = @"UploadPDA";
		public const string CustomerConfigHash = @"0";
		public const string CustomerConfigFileType = @"1";
		public const string CustomerConfigQType = @"0";
		public const string CustomerConfigUseAlphaKey = @"1";
		public const string CustomerConfigClientID = @"0";
		public const string CustomerConfigNewItem = @"0";
		public const string FolderErpExport = @"ExportData";
		public const string DebugDataPath = @"\..\..\..\Count4U.Configuration\DebugData";
		public const double HomeOpacityBackground = 0.2;
		public const double CustomerOpacityBackground = 0.2;
		public const double BranchOpacityBackground = 0.2;
		public const double InventorOpacityBackground = 0.2;
		public const double MainOpacityBackground = 0.2;
		public const string HomeBackgroundFilePath = @"C:\Count4U\trunk\Count4U\Count4U.Media\Background\Home.jpg";
		public const string BranchBackgroundFilePath = @"C:\Count4U\trunk\Count4U\Count4U.Media\Background\Branch.jpg";
		public const string InventorBackgroundFilePath = @"C:\Count4U\trunk\Count4U\Count4U.Media\Background\Inventor.jpg";
		public const string MainBackgroundFilePath = @"C:\Count4U\trunk\Count4U\Count4U.Media\Background\Main.jpg";
		public const string CustomerConfigIturNameType = @"ITUR";
		public const string CustomerConfigIturNamePrefix = @"ITUR";
		public const string CustomerBackgroundFilePath = @"C:\Count4U\trunk\Count4U\Count4U.Media\Background\Customer.jpg";
		public const int DBVer = 84;
		public const string ProductMakatBarcodesDictionaryCapacity = @"1000000";
		public const string ConnectionEFMaxDatabaseSize = @"2048";
		public const string ConnectionEFMaxBufferSize = @"4096";
		public const string CustomerConfigPassword = @"650";
		public const string MISiDnextDataPath = @"C:\MIS\IDnextData";
		public const string MISCommunicatorPath = @"C:\MIS\MISCommunicator";
		public const string CustomerConfigHTcalculateLookUp = @"No";
		public const string AddNewLocation = @"true";
		public const string MaxQuantity = @"999";
		public const string LastSync = @"DD-MM-YY HH:MM";
		public const string AllowZeroQuantity = @"false";
		public const string EmptyCount4MobileDBFile = @"Count4MobileDbTemplete.db3";
		public const string SearchDef = @"normal";
		public const string CustomerConfigNewItemBool = @"false";
		public const string AnalyticDBFile = @"AnalyticDB.sdf";
		public const string MaxLen = @"16";
		public const string CustomerConfigInvertPrefix = @"0";
		public const string ConfirmNewLocation = @"true";
		public const string ConfirmNewItem = @"false";
		public const string AutoSendData = @"0";
		public const string AllowQuantityFraction = @"true";
		public const string AddExtraInputValue = @"false";
		public const string AddExtraInputValueHeaderName = @"";
		public const string AddExtraInputValueSelectFromBatchListForm = @"false";
		public const string AllowNewValueFromBatchListForm = @"false";
		public const string SearchIfExistInBatchList = @"false";
		public const string AllowMinusQuantity = @"false";
		public const string FractionCalculate = @"false";
		public const string RootFolderFtp = @"mINV";
		public const string RootComplexDataFolderFtp = @"mINV\ComplexData";
		public const string PartialQuantity = @"false";
		public const string Host1 = @"192.168.100.1";
		public const string Host2 = @"192.168.100.20";
		public const string Timeout = @"10";
		public const string Retry = @"3";
		public const string DefaultHost = @"USB";
		public const string SameBarcodeInLocation = @"0";
		public const string ProcessDBFile = @"ProcessDB.sdf";
		public const string MonitorDBFile = @"MonitorDB.sdf";
		public const string EmptyAnalyticDBFile = @"EmptyAnalyticDB.sdf";
		public const string EmptyAnalyticDBPath = @"C:\Temp\testDB\App_Data\AnalyticDB.sdf";
		public const string EmptyAuditDBFile = @"EmptyAuditDB.sdf";
		public const string EmptyMainDBFile = @"EmptyMainDB.sdf";
		public const string DefaultFtpHost = @"ftp://ftp.boscom.com";//@"ftp://ftp.idnext.co.il";
		public const string DefaultUser = "idnext";
		public const string DefaultPassword = "ab1111!";

		//ftp://ftp.boscom.com/mINV/Customer/dn6x/Branch/dn6x-nry3/Inventor/2019/6/6/75c98290-7c77-4f08-8ddd-c90bbe106b45/Profile/
	}
}