using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Playground.Models
{
    public class SearchResult : INotifyPropertyChanged
    {
        private string _Text;
        private string _DisplayUrl; // I have found that some websites have a friendly display link.
        private string _Url;
        private string _Description;
        private string _Source;


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnNotifyPropertyChanged([CallerMemberName] string name = "")//By using CallerMemberName no need to hard code property names, reduces errors! Obtains method/property name of caller to the method
        {
            // the ? means if PropertyChanged is null it wont invoke.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public string Text 
        { 
            get { return _Text; } 
            set
            {
                _Text = value;
                OnNotifyPropertyChanged();
            }
        }
        public string Url {
            get { return _Url; }
            set
            {
                _Url = value;
                OnNotifyPropertyChanged();
            }
        }
        public string DisplayUrl
        {
            get { return _DisplayUrl; }
            set
            {
                _DisplayUrl = value;
                OnNotifyPropertyChanged();
            }
        }

        public string Description {
            get { return _Description; }
            set
            {
                _Description = value;
                OnNotifyPropertyChanged();
            }
        }

        public string Source {
            get { return _Source; }
            set
            {
                _Source = value;
                OnNotifyPropertyChanged();
            }
        }

        public SearchResult(string text, string displayUrl, string url, string description, string source)
        {
            Text = text;
            DisplayUrl = displayUrl;
            Url = url;
            Description = description;
            Source = source;
            Console.WriteLine(this.ToString());
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Text: " + this.Text);
            sb.AppendLine("Display Url: " + this.DisplayUrl);
            sb.AppendLine("Url: " + this.Url);
            sb.AppendLine("Description: " + this.Description);
            sb.AppendLine("Source: " + this.Source);
            return sb.ToString();
        }
    }
}
