using System;
using System.Collections.Generic;
using System.Speech.Recognition;
using System.Threading.Tasks;
using System.Windows;

namespace DensoBoxTester
{
    public static class SpeechRecognition
    {
        public static SpeechRecognitionEngine sre;
        public static double 一致率;

        public enum WORD { オッケー, キャンセル, エヌジー, カイシ, スタート, カクニン, NON }

        public static WORD word;

        static SpeechRecognition()
        {
            sre = new SpeechRecognitionEngine();

        }

        public static bool Init()
        {
            try
            {
                //入力ソース（既定のマイク）
                sre.SetInputToDefaultAudioDevice();//マイクが接続されていないとここで例外が発生する
                return true;
            }
            catch
            {
                return false;
            }

        }


        static bool Busy = false;
        public static void set()
        {
            //イベント登録
            sre.SpeechRecognized += async (object sender, SpeechRecognizedEventArgs e) =>
            {
                if (Busy)
                    return;

                Busy = true;

                try
                {
                    if (e.Result == null)
                    {
                        一致率 = 0;
                        return;
                    }

                    一致率 = e.Result.Confidence * 100;

                    if (一致率 < State.VmTestStatus.VoiceSpec)
                        return;

                    foreach (var w in Enum.GetValues(typeof(WORD)))
                    {
                        if (e.Result.Text == ((WORD)w).ToString())
                        {
                            word = (WORD)w;
                        }
                    }

                    switch (word)
                    {
                        case WORD.オッケー:
                        case WORD.カイシ:
                        case WORD.カクニン:
                        case WORD.スタート:
                            Flags.OkPushed = true;
                            break;
                        case WORD.エヌジー:
                        case WORD.キャンセル:
                            Flags.CancelPushed = true;
                            break;
                        default:
                            return;
                    }
                    State.VmTestStatus.音声認識率 = $"{ word.ToString()}  一致率{一致率.ToString("F0")}%";
                    Flags.FlagStopSwCheck = true;
                }
                finally
                {
                    await Task.Delay(250);
                    State.VmTestStatus.音声認識率 = "";
                    Busy = false;
                }
            };

        }

        public static void 音声認識()
        {
            Task.Run(() =>
            {
                try
                {
                    word = WORD.NON;//毎回初期化

                    var wordList = new List<string>();
                    foreach (var w in Enum.GetValues(typeof(WORD)))
                    {
                        wordList.Add(((WORD)w).ToString());
                    }

                    State.VmTestStatus.音声認識率 = "音声一致率 ----";
                    //語彙登録
                    Choices words = new Choices(wordList.ToArray());
                    sre.LoadGrammar(new Grammar(new GrammarBuilder(words)));

                    //入力ソース（既定のマイク）
                    sre.SetInputToDefaultAudioDevice();//マイクが接続されていないとここで例外が発生する


                    Flags.FlagStop音声認識 = false;
                    sre.RecognizeAsync(RecognizeMode.Multiple);//非同期で認識開始

                    while (true)
                    {
                        if (Flags.FlagStop音声認識)
                        {
                            sre.RecognizeAsyncCancel();
                            sre.RecognizeAsyncStop();
                            break;
                        }

                    }
                }
                catch
                {
                    MessageBox.Show("マイク接続異常\r\nアプリケーションを閉じます");
                    Environment.Exit(0);
                }
                finally
                {
                }

            });
        }

        public static void Close()
        {
            Flags.FlagStop音声認識 = true;
            sre.Dispose();
            sre = null;
        }
    }
}
