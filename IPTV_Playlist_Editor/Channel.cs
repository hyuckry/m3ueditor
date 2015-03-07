using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Kodi_M3U_IPTV_Editor
{
    public class Channel : INotifyPropertyChanged
    {

        private string _Name, _Group, _ip; //_epg, _image
        public event PropertyChangedEventHandler PropertyChanged;

        public Channel(int id, string Name, string ip)
        {
            _Name = Name;
            _ip = ip;

            //  _epg = epg;
        }
        public Channel(int id , string Group,string Name,  string ip)
        {
            _Name = Name;
            _Group = Group;
            _ip = ip;

          //  _epg = epg;
        }

      /*  public Channel(int id , string Group,string Name, string ip, string image)
        {

            _Group = Group;
            _Name = Name;

            _ip = ip;
        //    _epg = epg;
           // _image = image;
        }
        */
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                this.NotifyPropertyChanged("Name");
            }
        }

        public string Group
        {
            get { return _Group; }
            set
            {
                _Group = value;
                this.NotifyPropertyChanged("Group");
            }
        }

       

       

        public string IP
        {
            get { return _ip; }
            set
            {
                _ip = value;
                this.NotifyPropertyChanged("URL");
            }
        }

    
      //  public string EPG
     //   {
    //        get { return _epg; }
      //      set
          //  {
           //     _epg = value;
         //       this.NotifyPropertyChanged("EPG");
         //   }
        //}

      /*  public string Image
        {
            get { return _image; }
            set
            {
                _image = value;
                this.NotifyPropertyChanged("Image");
            }
        }
        */
        private void NotifyPropertyChanged(string value)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(value));
        }
    }
}
