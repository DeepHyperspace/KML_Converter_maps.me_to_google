using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Device.Location;

namespace GIS_Editor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //CivicAddressResolver resolver = new CivicAddressResolver();
            //GeoCoordinate latlon = new GeoCoordinate(12.30892, 51.344481);
            //CivicAddress address = resolver.ResolveAddress(latlon);
            //MessageBox.Show(address.CountryRegion, "Ergebnis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            GIS.ResolveAddressSync();

        }
    }


    class GIS
    {
        public static void ResolveAddressSync()
        {
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold = 1.0; // set to one meter
            watcher.TryStart(false, TimeSpan.FromMilliseconds(1000));

            CivicAddressResolver resolver = new CivicAddressResolver();

            if (watcher.Position.Location.IsUnknown == false)
            {
                GeoCoordinate latlon = new GeoCoordinate(12.30892, 51.344481);
                CivicAddress address = resolver.ResolveAddress(latlon);

                if (!address.IsUnknown)
                {
                    Console.WriteLine("Country: {0}, Zip: {1}",
                            address.CountryRegion,
                            address.PostalCode);
                    MessageBox.Show(address.CountryRegion, "Ergebnis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Console.WriteLine("Address unknown.");
                    MessageBox.Show(address.CountryRegion, "Ergebnis", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}

