using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ZSJCMaster.Helpers;
using ZSJCMaster.Models;

namespace ZSJCMaster.DB
{
    public class AlarmInfoOperator
    {
        private MySQLHelper helper;
        public AlarmInfoOperator()
        {
            XDocument doc = XDocument.Load("Application.config");
            var strs = doc.Descendants("connectionStrings");
            if (strs == null) { return; }
            var mysqlconn = strs.Descendants().SingleOrDefault(s=>s.Attribute("name").Value == "mysql");
            string connString;
            if (mysqlconn == null)
            {
                connString = "Server=127.0.0.1;Database=ZSJC;Uid=root;Pwd=123456;";
            }else
            {
                connString = mysqlconn.Attribute("connectionString").Value;
            }
            helper = new MySQLHelper(connString);
        }
         
        public List<AlarmInfoForDB> Query(DateTime? startTime,DateTime? endTime,int pageSize,int currentPageNum, out int totalRecordNum,int controlPadId = -1, int cameraId = -1)
        {
            StringBuilder sql = new StringBuilder($"select * from AlarmInfo where infoTime between '{startTime}' and '{endTime}' ");
            //如果控制板编号为-1，说明是查询所有控制板的记录
            if (controlPadId == -1)
            {

            }else
            {
                //查询指定控制板的记录
                sql.Append($" and controlpadId = {controlPadId}");
                //如果相机编号为-1，说明是查询所有相机的记录
                if(cameraId == -1)
                {

                }else
                {
                    //查询指定相机的记录
                    sql.Append($" and cameraId = {cameraId} ");
                }
            }
            //获得总页数
            totalRecordNum = helper.Query<AlarmInfoForDB>(sql.ToString()).Count();
            //继续查询
            sql.Append($" limit {(currentPageNum - 1) * pageSize},{pageSize}");
            var result = helper.Query<AlarmInfoForDB>(sql.ToString());
            
            return result.ToList<AlarmInfoForDB>();
        }

        public int Add(AlarmInfoForDB info)
        {
            string sql = "insert into alarminfo (ControlPadId,CameraId,X,Y,InfoTime) values (@ControlPadId,@CameraId,@X,@Y,@InfoTime)";
            return helper.Execute(sql, info);
        }

        public AlarmInfoForDB AlarmInfoToAlarmInfoForDB(int controlpadId,AlarmInfo info)
        {
            if (info == null) { return new AlarmInfoForDB(); }
            AlarmInfoForDB db = new AlarmInfoForDB()
            {
                ControlPadId = controlpadId,
                CameraId = info.CameraNo,
                X = info.X,
                Y = info.Y,
                InfoTime = DateTime.Parse(info.InfoTime)
            };
            return db;
        }
    }
}
