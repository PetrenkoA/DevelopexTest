using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace DevelopexTest
{
    public class Page : INotifyPropertyChanged
    {

        #region Properties

        int idValue;
        public int Id
        {
            get { return idValue; }
            set
            {
                if (idValue != value)
                {
                    idValue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        string UrlValue;
        public string Url
        {
            get { return UrlValue; }
            set
            {
                if (UrlValue != value)
                {
                    UrlValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string htmlValue;
        public string innerHTML
        {
            get { return htmlValue; }
            set
            {
                if (htmlValue != value)
                {
                    htmlValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        int wordCountValue;
        public int WordCount
        {
            get { return wordCountValue; }
            set
            {
                if (wordCountValue != value)
                {
                    wordCountValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        string StatusValue;
        public string Status
        {
            get { return StatusValue; }
            set
            {
                if (StatusValue != value)
                {
                    StatusValue = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged()
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Property changed"));
        }

        #endregion

        public Page(int id, string url)
        {
            this.Id = id;
            this.Url = url;
            Status = "Idle";
        }

        public int getWordCount(string wordToFind)
        {
            WordCount = (innerHTML.Length - innerHTML.Replace(wordToFind, "").Length) / wordToFind.Length;
            if (WordCount == 0) Status = "No matches found";
            else Status = string.Format("{0} matches found", WordCount);

            return WordCount;
        }

        public List<string> getPageURL(int maxUrls)
        {
            string ourUrl = string.Format("http://{0}", this.Url.Replace("http://", "").Split('/')[0]);
            List<string> result = new List<string>();
            while(result.Count < maxUrls && innerHTML.Contains(ourUrl))
            {
                int beg = innerHTML.IndexOf("http://");
                int end = innerHTML.Length;
                for(int i = beg + 7; i < innerHTML.Length; i++)
                {
                    if(!isUrlAllowedSymbol(innerHTML[i]))
                    {
                        end = i;
                        break;
                    }
                }
                result.Add(innerHTML.Substring(beg, end - beg));
                innerHTML = innerHTML.Substring(end);
            }
            return result;
        }

        private bool isUrlAllowedSymbol(char s)
        {
            if (Regex.IsMatch(s.ToString(), @"[\w-_.~!*'();:@&=+$,/?%#[]]*$")) return true;
            else return false;
        }

    }
}
