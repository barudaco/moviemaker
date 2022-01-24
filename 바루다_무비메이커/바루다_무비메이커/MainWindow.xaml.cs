using FFMediaToolkit.Decoding;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using 바루다_무비메이커.Commons;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;

namespace 바루다_무비메이커
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string PATH_FFMPEG_ROOT = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ffmpeg\x86_64\bin\");
        private static string PATH_FFMPEG_EXCUTE = Path.Combine(PATH_FFMPEG_ROOT, @"ffmpeg.exe");

        ViewModels.MainViewModel VM;
        public MainWindow()
        {
            InitializeComponent();
            FFMediaToolkit.FFmpegLoader.FFmpegPath = PATH_FFMPEG_ROOT;



            if (!Directory.Exists(Properties.Settings.Default.PATH_이미지폴더)) Properties.Settings.Default.PATH_이미지폴더 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(Properties.Settings.Default.PATH_오디오폴더)) Properties.Settings.Default.PATH_오디오폴더 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!Directory.Exists(Properties.Settings.Default.PATH_출력폴더)) Properties.Settings.Default.PATH_출력폴더 = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            this.VM = new ViewModels.MainViewModel();  // 뷰모델 생성
            this.DataContext = VM;
            VM.Append_Log("프로그램이 시작되었습니다.");

            InitBackgroundWorker();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // You can set there codec, bitrate, frame rate and many other options.
            var settings = new VideoEncoderSettings(width: 1920, height: 1080, framerate: 30, codec: VideoCodec.H264);
            settings.EncoderPreset = EncoderPreset.Fast;
            settings.CRF = 17;
            using (MediaOutput file = MediaBuilder.CreateContainer(@"D:\비디오생성\출력.mp4").WithVideo(settings).Create())
            {


                Bitmap bitmap = new Bitmap(@"D:\비디오생성\input\IMG_24.png");
                var rect = new Rectangle(Point.Empty, bitmap.Size);
                var bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);

                //for (var i = 0; i < 30; ++i)
                while (file.Video.CurrentDuration.TotalSeconds < 3)
                {
                    file.Video.AddFrame(bitmapData);
                }


                Bitmap bitmap2 = new Bitmap(@"D:\비디오생성\input\IMG_11566.png");
                var rect2 = new Rectangle(Point.Empty, bitmap2.Size);
                var bitLock2 = bitmap2.LockBits(rect2, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                var bitmapData2 = ImageData.FromPointer(bitLock2.Scan0, ImagePixelFormat.Bgr24, bitmap2.Size);

                while (file.Video.CurrentDuration.TotalSeconds < 3 + 3)
                {
                    file.Video.AddFrame(bitmapData2);
                }
            }
        }


        #region Setting UI
        private void 경로열기_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {           
            var ele = (sender as Hyperlink);
            if (null != ele)
            {
                var path = ele.NavigateUri.ToString();
                try
                {
                    Process.Start("explorer.exe", path);

                }catch (Exception err)
                {
                    MessageBox.Show(this, $"경로를 확인할 수 없습니다.\n{err.Message}\n{path}", "ERROR");
                }
            }            
        }

        private string lastSelectedPath;
        private void btn_경로변경_이미지_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if(!string.IsNullOrWhiteSpace(Properties.Settings.Default.PATH_이미지폴더))
                {
                    dialog.SelectedPath = Properties.Settings.Default.PATH_이미지폴더;
                }
                else
                {
                    dialog.SelectedPath = lastSelectedPath;
                }

                
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.PATH_이미지폴더 = dialog.SelectedPath;
                    Properties.Settings.Default.Save();

                    lastSelectedPath = dialog.SelectedPath;
                }
            }
        }

        private void btn_경로변경_오디오_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PATH_오디오폴더))
                {
                    dialog.SelectedPath = Properties.Settings.Default.PATH_오디오폴더;
                }
                else
                {
                    dialog.SelectedPath = lastSelectedPath;
                }

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.PATH_오디오폴더 = dialog.SelectedPath;
                    Properties.Settings.Default.Save();

                    lastSelectedPath = dialog.SelectedPath;
                }
            }
        }

        private void btn_경로변경_출력_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(Properties.Settings.Default.PATH_출력폴더))
                {
                    dialog.SelectedPath = Properties.Settings.Default.PATH_출력폴더;
                }
                else
                {
                    dialog.SelectedPath = lastSelectedPath;
                }
                
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Properties.Settings.Default.PATH_출력폴더 = dialog.SelectedPath;
                    Properties.Settings.Default.Save();

                    lastSelectedPath = dialog.SelectedPath;
                }
            }
        }

        private void IntegerUpDown_동영상길이_최소_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Properties.Settings.Default.동영상길이_최소 > Properties.Settings.Default.동영상길이_최대) Properties.Settings.Default.동영상길이_최대 = Properties.Settings.Default.동영상길이_최소;
            Properties.Settings.Default.Save();
        }

        private void IntegerUpDown_동영상길이_최대_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Properties.Settings.Default.동영상길이_최소 > Properties.Settings.Default.동영상길이_최대) Properties.Settings.Default.동영상길이_최소 = Properties.Settings.Default.동영상길이_최대;
            Properties.Settings.Default.Save();
        }

        private void IntegerUpDown_사진개수_최소_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Properties.Settings.Default.사진개수_최소 > Properties.Settings.Default.사진개수_최대) Properties.Settings.Default.사진개수_최대 = Properties.Settings.Default.사진개수_최소;
            Properties.Settings.Default.Save();
        }

        private void IntegerUpDown_사진개수_최대_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (Properties.Settings.Default.사진개수_최소 > Properties.Settings.Default.사진개수_최대) Properties.Settings.Default.사진개수_최소 = Properties.Settings.Default.사진개수_최대;
            Properties.Settings.Default.Save();
        }

        private void IntegerUpDown_동영상생성개수_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Properties.Settings.Default.Save();
        }
        #endregion

        private void btn_START_Click(object sender, RoutedEventArgs e)
        {
            Worker_StartWoker();
        }

        private void btn_STOP_Click(object sender, RoutedEventArgs e)
        {
            Worker_ExitWoker();
        }

        private void 작업로그창_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer scrollViewer && Math.Abs(e.ExtentHeightChange) > 0.0)
            {
                scrollViewer.ScrollToBottom();
            }
        }


        

        private void btn_movie2png_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string selectedFilePath;

                using (var dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    dialog.Multiselect = false;
                    dialog.Filter = "mp4 files (*.mp4)|*.mp4|MOV files (*.mov)|*.mov";
                    dialog.InitialDirectory = lastSelectedPath;


                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        selectedFilePath = dialog.FileName;
                        lastSelectedPath = Path.GetDirectoryName(selectedFilePath);
                    }
                    else
                    {
                        MessageBox.Show("취소되었습니다.");
                        return;
                    }
                }


                var i = 0;
                

                if(string.IsNullOrWhiteSpace(selectedFilePath))
                {
                    MessageBox.Show("동영상 파일을 선택해주세요.");
                    return;
                }
                


                if(!File.Exists(selectedFilePath))
                {
                    MessageBox.Show("동영상 파일이 없습니다.");
                    return;
                }

                if (!Directory.Exists(Properties.Settings.Default.PATH_출력폴더))
                {
                    MessageBox.Show("출력 경로를 확인해주세요.");
                    return;
                }

                var orgFileName = Path.GetFileNameWithoutExtension(selectedFilePath);

                var targetFolderPath = Properties.Settings.Default.PATH_출력폴더;
                if(!Directory.Exists(targetFolderPath))
                {
                    Directory.CreateDirectory(targetFolderPath);
                }
                



                var file = MediaFile.Open(selectedFilePath);
                var frameCount = 0;

                VM.Append_Log("[movie to png] 시작");
                VM.Append_Log($"[movie to png] 총 길이 {file.Info.Duration.TotalSeconds} 초");



                try
                {
                    while (file.Video.TryGetNextFrame(out var imageData))
                    {
                        if (0 == frameCount % 30)
                        {
                            Utils.ToBitmap(imageData).Save(Path.Combine(targetFolderPath, $@"{orgFileName}_{i++}.png"));
                        }
                        frameCount++;
                    }
                }catch(Exception err)
                {
                    VM.Append_Error("[movie to png] 오류");
                    VM.Append_Error(err.Message);
                }
                

                VM.Append_Blue("[movie to png] 완료");
            }
            catch(Exception err)
            {
                VM.Append_Error("[movie to png] 오류");
                VM.Append_Error(err.Message);
            }
        }

    }

    partial class MainWindow
    {
        #region 백그라운드워커 설정
        //========================================================= 백그라운드 워커
        // 백그라운드 작업 워커
        private BackgroundWorker _worker = null;

        // START 백그라운드 워커
        public void InitBackgroundWorker()
        {
            Worker_ExitWoker();
            _worker = new BackgroundWorker();
            _worker.WorkerSupportsCancellation = true;
            _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
            _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerComplete);
        }

        public void Worker_StartWoker()
        {
            //_worker 객체가 생성된게 있다면
            if (_worker != null)
            {
                //해당 스레드 객체가 Busy 상태이면
                if (!_worker.IsBusy)
                {
                    //스레드 시작
                    _worker.RunWorkerAsync();
                }
            }
        }

        public void Worker_ExitWoker()
        {
            //_worker 객체가 생성된게 있다면
            if (_worker != null)
            {
                //해당 스레드 객체가 Busy 상태이면
                if (_worker.IsBusy)
                {
                    //스레드 취소
                    _worker.CancelAsync();

                }
            }
        }

        // 워커 종료 메서드
        public void Worker_RunWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            //Error 체크
            if (e.Cancelled)    // 사용자가 취소한 경우
            {
                this.VM.Append_Error("[작업종료] 사용자가 작업을 취소하였습니다.");
            }
            else if (e.Error != null)
            {
                string msg = string.Format("ERROR : {0}", e.Error.Message);
                this.VM.Append_Error("[작업종료] " + msg);
            }
            else
            {
                try
                {
                    this.VM.Append_Blue("[작업종료] 완료");
                }
                catch (Exception err)
                {
                    this.VM.Append_Error("[작업종료] 완료(오류) - " + err.Message);
                }
            }
            _worker.Dispose();
        }



        #endregion

        public void Worker_DoWork(object sender, DoWorkEventArgs e)
        {            
            string PATH_이미지폴더 = Properties.Settings.Default.PATH_이미지폴더;
            string PATH_오디오폴더 = Properties.Settings.Default.PATH_오디오폴더;
            string PATH_출력폴더 = Properties.Settings.Default.PATH_출력폴더;
            int 동영상길이_최소 = Properties.Settings.Default.동영상길이_최소;
            int 동영상길이_최대 = Properties.Settings.Default.동영상길이_최대;
            int 사진개수_최소 = Properties.Settings.Default.사진개수_최소;
            int 사진개수_최대 = Properties.Settings.Default.사진개수_최대;
            int 동영상생성개수 = Properties.Settings.Default.동영상생성개수;


            List<string> 이미지파일경로목록 = new List<string>();
            List<string> 오디오파일경로목록 = new List<string>();


            if (!Directory.Exists(PATH_이미지폴더)) throw new Exception("[경로오류] 이미지 폴더 경로가 잘못 되었습니다.");
            if (!Directory.Exists(PATH_오디오폴더)) throw new Exception("[경로오류] 오디오 폴더 경로가 잘못 되었습니다.");
            if (!Directory.Exists(PATH_출력폴더)) throw new Exception("[경로오류] 출력 폴더 경로가 잘못 되었습니다.");

            try
            {
                이미지파일경로목록 = Directory.GetFiles(PATH_이미지폴더, "*.*", SearchOption.AllDirectories).Where(s => "*.jpg,*.png".Contains(System.IO.Path.GetExtension(s).ToLower())).ToList();
            }
            catch(Exception err)
            {
                this.VM.Append_Error($"[오류] 사진 파일을 확인할 수 없습니다.");
                throw err;
            }

            try
            {
                오디오파일경로목록 = Directory.GetFiles(PATH_오디오폴더, "*.*", SearchOption.AllDirectories).Where(s => "*.mp3".Contains(System.IO.Path.GetExtension(s).ToLower())).ToList();
            }
            catch (Exception err)
            {
                this.VM.Append_Error($"[오류] 오디오 파일을 확인할 수 없습니다.");
                throw err;
            }

            if (0 == 이미지파일경로목록.Count) throw new Exception("[파일확인] 이미지 파일이 없습니다. 폴더를 확인 해주세요. (*.jpg *.png) 파일만 지원합니다.");


            this.VM.Append_Blue($"[작업시작] 총 ({동영상생성개수})개의 동영상을 생성 합니다. 총 ({이미지파일경로목록.Count})개의 이미지가 사용 됩니다.");

            for (var i = 0; i < 동영상생성개수; ++i)
            {
                // 사용자 작업 중지 
                if (_worker.CancellationPending && _worker.IsBusy)  
                {
                    e.Cancel = true;
                    return;
                }

                
                var 동영상길이_초 = Utils.Random(동영상길이_최소, 동영상길이_최대);
                List<string> 이미지파일목록 = new List<string>();
                for(var j = 0; j < Utils.Random(사진개수_최소, 사진개수_최대); ++j)
                {
                    이미지파일목록.Add(이미지파일경로목록[Utils.Random(0, 이미지파일경로목록.Count - 1)]);
                }

                //var 출력파일경로 = Path.Combine(PATH_출력폴더, $"{DateTime.Now.Ticks}.mp4");
                var 출력파일경로_TEMP = Path.Combine(PATH_출력폴더, $"{Path.GetRandomFileName().Replace(".", "")}.mp4");
                var 출력파일경로 = Path.Combine(PATH_출력폴더, $"{Path.GetRandomFileName().Replace(".","")}.mp4");
                int DurationSec = (int)(동영상길이_초 / 이미지파일목록.Count);


                this.VM.Append_Log($"[{i + 1}/{동영상생성개수}] [영상] 생성중.");                

                try
                {
                    var settings = new VideoEncoderSettings(width: 1920, height: 1080, framerate: 30, codec: VideoCodec.H264);
                    settings.EncoderPreset = EncoderPreset.Fast;
                    settings.CRF = 17;

                                       
                    using (MediaOutput file = MediaBuilder.CreateContainer(출력파일경로_TEMP).WithVideo(settings).Create())
                    {
                        try
                        {
                            var currentDurationSec = 0;
                            foreach (var 이미지파일경로 in 이미지파일목록)
                            {
                                currentDurationSec += DurationSec;
                                Bitmap bitmap = new Bitmap(이미지파일경로);
                                var rect = new Rectangle(Point.Empty, bitmap.Size);
                                var bitLock = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                                var bitmapData = ImageData.FromPointer(bitLock.Scan0, ImagePixelFormat.Bgr24, bitmap.Size);

                                //for (var i = 0; i < 30; ++i)
                                while (file.Video.CurrentDuration.TotalSeconds < currentDurationSec)
                                {
                                    file.Video.AddFrame(bitmapData);
                                }

                                this.VM.AppendLast_Log_String(".");
                            }
                        }catch(Exception err)
                        {
                            this.VM.RemoveLast_Log();
                            this.VM.Append_Error($"[{i + 1}/{동영상생성개수}] 생성실패 - 영상 생성 오류 - {err.Message}");
                        }
                    }
                            
                    
                    if(File.Exists(출력파일경로_TEMP))
                    {

                        if (0 < 오디오파일경로목록.Count)
                        {
                            this.VM.AppendLast_Log_String("OK / [오디오] 입력중...");
                            var 오디오파일경로 = 오디오파일경로목록[Utils.Random(0, 오디오파일경로목록.Count - 1)];
                            try
                            {
                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                startInfo.FileName = PATH_FFMPEG_EXCUTE;
                                startInfo.Arguments = string.Format(" -i \"{0}\" -i \"{1}\" -shortest -y \"{2}\"", 출력파일경로_TEMP, 오디오파일경로, 출력파일경로);
                                startInfo.CreateNoWindow = true;                               // cmd창을 띄우지 안도록 하기
                                startInfo.UseShellExecute = false;

                                using (Process exeProcess = Process.Start(startInfo))
                                {
                                    exeProcess.WaitForExit();
                                }

                                if (File.Exists(출력파일경로_TEMP))
                                {
                                    File.Delete(출력파일경로_TEMP);
                                }

                                this.VM.AppendLast_Log_String($"OK / [결과] 길이 ({동영상길이_초})초, 사진 ({이미지파일목록.Count})개, 사진당 ({동영상길이_초 / 이미지파일목록.Count})초, {출력파일경로}");
                            }
                            catch (Exception err)
                            {
                                this.VM.RemoveLast_Log();
                                this.VM.Append_Error($"[{i + 1}/{동영상생성개수}] 생성실패 - 배경음악 생성 오류 - {err.Message}");
                            }
                        }
                    }

                    
                }
                catch(Exception err)
                {
                    this.VM.RemoveLast_Log();
                    this.VM.Append_Error($"[{i + 1}/{동영상생성개수}] 생성실패 - {err.Message}");
                }                
            }
        }
    }
}
