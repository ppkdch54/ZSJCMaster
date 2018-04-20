using Prism.Mvvm;
using System;
using ZSJCMaster.Helpers;

namespace ZSJCMaster.Models
{
    interface ICamera
    {
        void StartDetect();
        void StopCap();
        void ShowVideo();
        void CapturePic();
    }
    public class Camera : BindableBase,ICamera
    {
        /// <summary>
        /// 相机编号
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// 相机名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 检测初始X坐标
        /// </summary>
        public int StartPointX { get; set; }
        /// <summary>
        /// 检测初始Y坐标
        /// </summary>
        public int StartPointY { get; set; }
        /// <summary>
        /// 检测矩形宽度
        /// </summary>
        public int RectWidth { get; set; }
        /// <summary>
        /// 检测矩形高度
        /// </summary>
        public int RectHeight { get; set; }
        /// <summary>
        /// 检测左边界
        /// </summary>
        public int LeftX { get; set; }
        /// <summary>
        /// 检测右边界
        /// </summary>
        public int RightX { get; set; }
        /// <summary>
        /// 检测上边界
        /// </summary>
        public int UpY { get; set; }
        /// <summary>
        /// 检测下边界
        /// </summary>
        public int DownY { get; set; }
        /// <summary>
        /// 开始指定y坐标
        /// </summary>
        public int StartDY { get; set; }
        /// <summary>
        /// 纵撕报警长度（m）
        /// </summary>
        public int AlarmLength { get; set; }
        /// <summary>
        /// 纵撕停带长度（m）
        /// </summary>
        public int StopLength { get; set; }
        /// <summary>
        /// 报警撕伤深度（像素）
        /// </summary>
        public int AlarmDepth { get; set; }
        /// <summary>
        /// 停带撕伤深度（像素）
        /// </summary>
        public int StopDepth { get; set; }
        /// <summary>
        /// 报警撕伤宽度（像素）
        /// </summary>
        public int AlarmWidth { get; set; }
        /// <summary>
        /// 停带撕伤宽度（像素）
        /// </summary>
        public int StopWidth { get; set; }
        private string cameraPicsDirPath;
        /// <summary>
        /// 相片抓拍目录
        /// </summary>
        public string CameraPicsDirPath
        {
            get { return cameraPicsDirPath; }
            set
            {
                cameraPicsDirPath = value;
                this.RaisePropertyChanged("CameraPicsDirPath");
            }
        }

        public Camera()
        {
            LoadPara();
        }

        private void LoadPara()
        {
            XmlConfigHelper config = new XmlConfigHelper();
            config.Load("Application.config");
            No = int.Parse(config.ReadNodeValue("no"));
            StartPointX = int.Parse(config.ReadNodeValue("startPointX"));
            StartPointY = int.Parse(config.ReadNodeValue("startPointY"));
            RectWidth = int.Parse(config.ReadNodeValue("rectWidth"));
            RectHeight = int.Parse(config.ReadNodeValue("rectHeight"));
            LeftX = int.Parse(config.ReadNodeValue("leftX"));
            RightX = int.Parse(config.ReadNodeValue("rightX"));
            UpY = int.Parse(config.ReadNodeValue("upY"));
            DownY = int.Parse(config.ReadNodeValue("downY"));
            StartDY = int.Parse(config.ReadNodeValue("startDY"));
            CameraPicsDirPath = config.ReadNodeValue("cameraPicsDirPath");
        }

        public void StopCap()
        {
            throw new NotImplementedException();
        }

        public void ShowVideo()
        {
            throw new NotImplementedException();
        }

        public void CapturePic()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 开始检测
        /// </summary>
        public void StartDetect()
        {
            //将图片存入临时变量
 
        }



    }
}
