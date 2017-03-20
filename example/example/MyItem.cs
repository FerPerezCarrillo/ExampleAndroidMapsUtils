using System;
using Android.Gms.Maps.Model;
using Com.Google.Maps.Android.Clustering;

namespace example
{
	public class MyItem: Java.Lang.Object,IClusterItem
	{
		public LatLng Position { get; set; }
		public string Title { get; set; }
		public int IconType;

		public MyItem(double lat, double lng, string title, int iconType)
		{
			Position = new LatLng(lat, lng);
			Title = title;
			IconType = iconType;
		}
	}
}
