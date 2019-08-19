using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZennMusic.Enums;
using ZennMusic.Managers;
using ZennMusic.Models;
using CommandManager = System.Windows.Input.CommandManager;

namespace ZennMusic
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SheetManager.Initialize();
            ChatManager.Initialize();
            LogManager.Initialize();

            Initialize();
            InitializeLog();
        }

        private void Initialize()
        {
            SongRequestListBox.ItemsSource = SongManager.SongList;

            SongManager.SongList.CollectionChanged += (sender, e)
                => SongCountText.Text = $"현재 {SongManager.SongList.Count}개의 곡이 신청되었습니다.";

            FontComboBox.SelectedIndex = FontComboBox.Items.Cast<FontFamily>()
                .ToList()
                .FindIndex(x => x.FamilyNames.Select(n => n.Value.ToLower().Replace(" ", "")).Contains("나눔고딕"));
        }

        private void InitializeLog()
        {
            AppDomain.CurrentDomain.UnhandledException += (e, arg) =>
            {
                var ex = arg.ExceptionObject as Exception;
                LogManager.Log(ex?.Message);
                LogManager.Log(ex?.StackTrace, false);
            };
        }

        private void OnFontSelected(object sender, SelectionChangedEventArgs e) 
            => Resources["MyFont"] = FontComboBox.SelectedItem as FontFamily;

        private void OnRequestToggleChecked(object sender, RoutedEventArgs e) 
            => SongManager.IsRequestAvailable = true;

        private void OnRequestToggleUnchecked(object sender, RoutedEventArgs e) 
            => SongManager.IsRequestAvailable = false;

        private void OnNextSongButtonClick(object sender, RoutedEventArgs e)
        {
            if (SongManager.SongList.Count <= 0)
                return;

            SongManager.SongList.Add(SongManager.SongList[0]);
            SongManager.SongList.RemoveAt(0);
        }

        private void OnSongDelete(object sender, RoutedEventArgs e)
            => RemoveSelectedSong();

        private void OnSongRefund(object sender, RoutedEventArgs e)
        {
            var song = RemoveSelectedSong();

            if (song == null)
                return;

            var point = SheetManager.GetUserPoint(song.Requester);
            switch (song.Type)
            {
                case RequestType.Piece:
                    SheetManager.SetPiece(song.Requester, point.Piece + 3);
                    break;
                case RequestType.Ticket:
                    SheetManager.SetTicket(song.Requester, point.Ticket + 1);
                    break;
            }
        }

        private Song RemoveSelectedSong()
        {
            if (SongRequestListBox.SelectedIndex == -1)
                return null;
            if (!(SongRequestListBox.SelectedItems is Song selectedItem))
                return null;

            SongManager.SongList.Remove(selectedItem);

            return selectedItem;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.W)
                    OnMoveKeyDown(true);
                else if (e.Key == Key.S)
                    OnMoveKeyDown(false);
            }
            if (e.Key == Key.Enter)
                OnCustomSongButtonClick(null, null);
        }

        private void OnMoveKeyDown(bool isUp)
        {
            if (SongRequestListBox.SelectedItem == null || !(SongRequestListBox.SelectedItems is Song request))
                return;

            var index = SongManager.SongList.IndexOf(request);
            var newIndex = index + (isUp ? 1 : -1);

            if (newIndex < 0) return;
            if (newIndex >= SongManager.SongList.Count) return;

            SongManager.SongList.Move(index, newIndex);
        }

        private void OnCustomSongButtonClick(object sender, RoutedEventArgs e)
        {
            if (CustomInputBox.Text == string.Empty)
                return;

            SongManager.SongList.Add(new Song(CustomInputBox.Text, "젠", RequestType.Special));
            CustomInputBox.Text = string.Empty;
        }
    }
}
