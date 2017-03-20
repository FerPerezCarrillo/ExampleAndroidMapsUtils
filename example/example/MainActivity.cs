using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Runtime;

using Java.Lang;

using Com.Google.Maps.Android.Clustering;
using Com.Google.Maps.Android.Clustering.View;
using Android.Content;
using Android.Views;

namespace example
{
	[Activity(Label = "example", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity, IOnMapReadyCallback, ClusterManager.IOnClusterClickListener,
	ClusterManager.IOnClusterItemClickListener, ClusterManager.IOnClusterItemInfoWindowClickListener
	{
		private GoogleMap _map;
		private MapFragment _mapFragment;
		ClusterManager _clusterManager;
		double Longitude = -103.3496;
		double Latitude = 20.6597;
		int Zoom = 12;
		
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			MapsInitializer.Initialize(this);
			
			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			InitMapFragment();
		}

		private void InitMapFragment()
		{
			_mapFragment = FragmentManager.FindFragmentByTag("map") as MapFragment;
			if (_mapFragment == null)
			{
				GoogleMapOptions mapOptions = new GoogleMapOptions()
					.InvokeMapType(GoogleMap.MapTypeSatellite)
					.InvokeZoomControlsEnabled(true)
					.InvokeCompassEnabled(true)
					.InvokeMapType(GoogleMap.MapTypeNormal)
					.InvokeZoomGesturesEnabled(true)
					.InvokeRotateGesturesEnabled(false);

				FragmentTransaction fragTx = FragmentManager.BeginTransaction();
				_mapFragment = MapFragment.NewInstance(mapOptions);
				fragTx.Add(Resource.Id.map, _mapFragment, "map");
				fragTx.Commit();

			}
			_mapFragment.GetMapAsync(this);
		}

		public void OnMapReady(GoogleMap googleMap)
		{
			_map = googleMap;
			_map.MyLocationEnabled = true;
			SetViewPoint();
			SetupMapCluster();
		}

		private void SetupMapCluster()
		{
			if (_map != null)
			{

				_clusterManager = new ClusterManager(this, _map);
				_clusterManager.Renderer = new MarkerRenderer(this, _map, _clusterManager);
				_clusterManager.SetOnClusterClickListener(this);
				_clusterManager.SetOnClusterItemClickListener(this);
				_clusterManager.SetOnClusterItemInfoWindowClickListener(this);
				_clusterManager.MarkerCollection.SetOnInfoWindowAdapter(new CustomInfoAdapter(LayoutInflater.From(this)));

				_map.SetOnCameraIdleListener(_clusterManager);
				_map.SetOnMarkerClickListener(_clusterManager);
				_map.SetOnInfoWindowClickListener(_clusterManager);
				_map.SetInfoWindowAdapter(_clusterManager.MarkerManager);

				AddClusterItems();
				_clusterManager.Cluster();
			}
		}

		public void SetViewPoint()
		{
			LatLng location = new LatLng(Latitude, Longitude);
			CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
			builder.Target(location);
			builder.Zoom(Zoom);
			CameraPosition cameraPosition = builder.Build();
			CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);
			_map.MoveCamera(cameraUpdate);
		}

		private void AddClusterItems()
		{
			for (int i = 0; i < 20; i++)
			{
				var item = new MyItem(Latitude, Longitude,"Pin: "+i, GetRandom());
				Latitude += 0.002;
				Longitude += 0.001;
				_clusterManager.AddItem(item);
			}
		}

		public bool OnClusterClick(ICluster cluster)
		{
			Toast.MakeText(this, cluster.Size + " Centros", ToastLength.Short).Show();
			return false;
		}

		public bool OnClusterItemClick(Java.Lang.Object p0)
		{
			return false;
		}

		public void OnClusterItemInfoWindowClick(Java.Lang.Object item)
		{
			var currentMarker = item.JavaCast<MyItem>();
			Toast.MakeText(this,"Title: " + currentMarker.Title, ToastLength.Short).Show();
		}

		private int GetRandom()
		{
			Random rnd = new Random();
			int Value = rnd.Next(1, 4);
			Console.WriteLine(Value);
			return Value;
		}
		#region ClusterMarkerRenderer
		public class MarkerRenderer : DefaultClusterRenderer
		{
			public MarkerRenderer(Context context, GoogleMap map, ClusterManager manager) : base(context, map, manager)
			{
			}

			protected override void OnBeforeClusterItemRendered(Java.Lang.Object item, MarkerOptions markerOptions)
			{
				var clusterItem = item.JavaCast<MyItem>();

				BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(GetResourceForPin(clusterItem.IconType));
				markerOptions.SetIcon(icon).SetTitle(clusterItem.Title);
			}

			protected override void OnBeforeClusterRendered(ICluster cluster, MarkerOptions markerOptions)
			{
				base.OnBeforeClusterRendered(cluster, markerOptions);
			}

			protected override bool ShouldRenderAsCluster(ICluster cluster)
			{
				return cluster.Size > 1;
			}

			private int GetResourceForPin(int tipo)
			{
				int resource = 0;
				switch (tipo)
				{
					case 1:
						resource = Resource.Drawable.pin1;
						break;
					case 2:
						resource = Resource.Drawable.pin2;
						break;
					case 3:
						resource = Resource.Drawable.pin3;
						break;
				}
				return resource;
			}
		}

		public class CustomInfoAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
		{
			private LayoutInflater _layoutInflater = null;

			public CustomInfoAdapter(LayoutInflater inflater)
			{
				_layoutInflater = inflater;

			}

			public View GetInfoContents(Marker marker)
			{
				var customPopup = _layoutInflater.Inflate(Resource.Layout.CustomInfoWindow, null);
				string title = System.String.Empty;

				title = marker.Title;

				var titleTextView = customPopup.FindViewById<TextView>(Resource.Id.TitleMarker);

				if (titleTextView != null)
				{
					titleTextView.Text = title;
				}

				return customPopup;
			}

			public View GetInfoWindow(Marker marker)
			{
				return null;
			}
		}
		#endregion
	}
}