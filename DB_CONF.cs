using System.Text;

namespace CS_MYSQL_DATA
{
    public class DB_CONF : DB_INFO
    {
        // new CONFIG(string "설정 파일명", string "폴더명:기본값etc")
        CONFIG config = new CONFIG("DB.ini");

        // 설정파일 항목([섹션])
        public string section;

        // 설정파일 항목(키=기본값)
        public bool used = true;
        public string comment = "DB서버";

        public override string Ip { get; set; } = "localhost";
        public override string Port { get; set; } = "3306";
        public override string Dbname { get; set; } = "";
        public override string Id { get; set; } = "";
        public override string Pw { get; set; } = "";
        public override string Charset { get; set; } = ""; // Using Default
        public override string Timeout { get; set; } = ""; // Mysql Default 15

        public string strConn // 접속문자열 생성
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                if (Ip != "") sb.Append(string.Format("SERVER={0};", Ip));
                if (Port != "") sb.Append(string.Format("PORT={0};", Port));
                if (Dbname != "") sb.Append(string.Format("Database={0};", Dbname));
                if (Id != "") sb.Append(string.Format("Uid={0};", Id));
                if (Pw != "") sb.Append(string.Format("Password={0};", Pw));
                if (Charset != "") sb.Append(string.Format("Charset={0};", Charset));
                if (Timeout != "") sb.Append(string.Format("Connection Timeout={0};", Timeout));

                return sb.ToString();
            }
        }

        /// <summary>
        /// 특정 SECTION 전용 생성자
        /// </summary>
        /// <param name="value">[SECTION 명]</param>
        public DB_CONF(string value = "LOCAL")
        {
            section = value;

            // 초기값 설정
            switch (section)
            {
                case "LOCAL": // 로컬서버
                    used = true;
                    comment = "DB(로컬)";
                    Ip = "localhost";
                    Port = "3306";
                    Dbname = "warning";
                    Id = "WBEarly";
                    Pw = "#woobosys@early!";
                    Timeout = "5";
                    break;
                case "IDC": // 예경보서버
                    used = true;
                    comment = "DB(IDC)";
                    Ip = "211.34.105.29";
                    Port = "4097";
                    Dbname = "weathersi";
                    Id = "WBEarly";
                    Pw = "#woobosys@early!";
                    Timeout = "5";
                    break;
                case "WAS": // WAS 서버
                    used = false;
                    comment = "DB(WAS)";
                    Ip = "localhost";
                    Dbname = "warning";
                    Id = "WeatherSI";
                    Pw = "@weathersi#";
                    Timeout = "5";
                    break;
                case "NDMS": // NDMS 서버
                    used = true;
                    comment = "DB(NDMS)";
                    Ip = "localhost";
                    Dbname = "ndms";
                    Id = "WBEarly";
                    Pw = "#woobosys@early!";
                    Timeout = "5";
                    break;
                case "OP": // 온품 서버
                    used = true;
                    comment = "DB(OP)";
                    Ip = "localhost";
                    Dbname = "ews";
                    Id = "ewsWoobo";
                    Pw = "ewsWoobo12#";
                    Timeout = "5";
                    break;
                case "GWD": // 강원도청 서버
                    used = true;
                    comment = "DB(GWD)";
                    Ip = "1.215.113.218";
                    Dbname = "weathersi";
                    Id = "WeatherSI";
                    Pw = "@weathersi#";
                    Timeout = "5";
                    break;
            }

            ReadConfig();
        }

        public bool ReLoad()
        {
            return ReadConfig(true);
        }

        public bool ReadConfig()
        {
            return ReadConfig(false);
        }

        public bool ReadConfig(bool isReset)
        {
            // 현재 파일의 기록시간 과 마지막 읽은 파일의 기록시간 비교
            if (config.LastWriteTime == config.LastReadTime)
            {
                // 파일의 변경이 없으면 false
                if (isReset == false)
                    return false;
            }

            // 동시접근제어 잠금(PC상의 모든 프로세스에서 하나만 접근 가능)
            if (config.LockMutex())
            {
                config.LastReadTime = config.LastWriteTime;

                // config.ReadXxxx(섹션, 키, 기본값)
                used = config.ReadBool(section, "USED", used);
                comment = config.ReadString(section, "COMMENT", comment);
                Ip = config.ReadString(section, "IP", Ip);
                Port = config.ReadString(section, "PORT", Port);
                Dbname = config.ReadString(section, "DBNAME", Dbname);
                Id = config.ReadString(section, "ID", Id);
                Pw = config.ReadPassword(section, "PW", Pw);
                Charset = config.ReadString(section, "CHARSET", Charset);
                Timeout = config.ReadString(section, "TIMEOUT", Timeout);

                // 동시접근제어 해제(PC상의 모든 프로세스에서 하나만 접근 가능)
                config.ReleaseMutex();
            }

            // 파일의 변경이 있으면(설정파일이 갱신 되었으면) true
            return true;
        }

        /// <summary>
        /// 특정 SECTION 조회
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool ReadConfig(string value)
        {
            section = value;

            return ReadConfig();
        }

        public void SaveConfig()
        {
            // 동시접근제어 잠금(PC상의 모든 프로세스에서 하나만 접근 가능)
            if (config.LockMutex())
            {
                //섹션
                config.WriteBool(section, "USED", used);
                config.WriteString(section, "COMMENT", comment);
                config.WriteString(section, "IP", Ip);
                config.WriteString(section, "PORT", Port);
                config.WriteString(section, "DBNAME", Dbname);
                config.WriteString(section, "ID", Id);
                config.WritePassword(section, "PW", Pw);
                config.WriteString(section, "CHARSET", Charset);
                config.WriteString(section, "TIMEOUT", Timeout);

                // 마지막 파일의 기록시간 변경
                config.LastReadTime = config.LastWriteTime;

                // 동시접근제어 해제(PC상의 모든 프로세스에서 하나만 접근 가능)
                config.ReleaseMutex();
            }

            return;
        }
    }

    public class DB_INFO
    {
        public virtual string Ip { get; set; } = "";
        public virtual string Port { get; set; } = "";
        public virtual string Dbname { get; set; } = "";
        public virtual string Id { get; set; } = "";
        public virtual string Pw { get; set; } = "";
        public virtual string Charset { get; set; } = "";
        public virtual string Timeout { get; set; } = "";


        public DB_INFO()
        {

        }

        public DB_INFO(string ip, string port, string dbname, string id, string pw, string charset = "", string timeout = "5")
        {
            Ip = ip;
            Port = port;
            Dbname = dbname;
            Id = id;
            Pw = pw;
            Charset = charset;
            Timeout = timeout;
        }
    }
}
