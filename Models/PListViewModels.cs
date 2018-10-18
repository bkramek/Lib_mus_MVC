using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StronaMuzy.Models
{

    public class PListViewModels
    {
        public Playlista Playlist { get; set; }
        public IEnumerable<SelectListItem> AllSongs{ get; set; }

        private List<int> _selectedSongs;
        public List<int> SelectedSongs
        {
            get
            {
                if (_selectedSongs == null)
                {
                    _selectedSongs = Playlist.Songs.Select(m => m.id).ToList();
                }
                return _selectedSongs;
            }

            set { _selectedSongs = value; }
        }
    }
    public class PsongList
    {
        public Song Sonk { get; set; }
        public IEnumerable<SelectListItem> AllPlaylists { get; set; }
        public Playlista Playlist { get; set; }
        private List<int> _selectedPlaylists;
        public List<int> SelectedPlaylists
        {
            get
            {
                if (_selectedPlaylists == null)
                {
                    _selectedPlaylists = Sonk.Playlistas.Select(m => m.id).ToList();
                }
                return _selectedPlaylists;
            }
            set { _selectedPlaylists = value; }
        }
    }
}