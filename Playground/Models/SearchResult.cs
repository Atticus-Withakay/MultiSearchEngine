using System;
using System.ComponentModel;
using System.Text;

namespace Playground.Models
{
    public class SearchResult : Notifier, INotifyPropertyChanged
    {
        private string _Text;
        private string _DisplayUrl; // I have found that some websites have a friendly display link.
        private string _Url;
        private string _Description;
        private string _Source;

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
