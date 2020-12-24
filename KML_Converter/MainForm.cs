using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wibci.CountryReverseGeocode;
using Wibci.CountryReverseGeocode.Models;
using System.Xml;
using System.Xml.Linq;

namespace GIS_Editor
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private CountryReverseGeocodeService gServce = new CountryReverseGeocodeService();


        List<XmlNode> PlacemarkList = new List<XmlNode>();
        List<XmlNode> OtherNodeList = new List<XmlNode>();
        string ResultPath = Application.StartupPath + "/Result/";

        XmlNode xmlDocNode;
        private void bt_load_Click(object sender, EventArgs e)
        {
            openKml.InitialDirectory = Application.StartupPath;
            openKml.Filter = "KML Files (*.kml)|*.kml";
            openKml.FilterIndex = 1;
            openKml.Multiselect = false;
            openKml.RestoreDirectory = true;
            openKml.ShowDialog();

            string filename = openKml.FileName;
            if (System.IO.File.Exists(filename))
            {
                xmlDocNode = LoadXML(filename);
            }

            if (xmlDocNode != null)
            {
                var msg = "Placemarks: " + Convert.ToString(PlacemarkList.Count) + Environment.NewLine +
                "ChildNodes: " + Convert.ToString(xmlDocNode.ChildNodes.Count) + Environment.NewLine +
                "OtherNodes: " + Convert.ToString(OtherNodeList.Count);

                //MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lb_xmlstats.Text = msg;
            }

        }

        private void bt_split_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.GetFiles(ResultPath, "*.kml").Length == 0)
            {
                WriteXML(PlacemarkList, Convert.ToInt32(numericUpDown1.Value));
            }
            else
            {
                MessageBox.Show("Please delete all *.kml files in the 'Result' folder and try again.", "Attention", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //has matching *.wma files
            }


        }

        public XmlNode LoadXML(string filename)
        {
            try
            {
                PlacemarkList.Clear();
                OtherNodeList.Clear();
                var xDocument = XDocument.Load(filename);
                string sXML = xDocument.ToString();
                System.Xml.XmlDocument xml = new System.Xml.XmlDocument();
                xml.LoadXml(sXML);
                XmlNode topnode = xml.DocumentElement;


                if (topnode.Name == "kml" && topnode.FirstChild.Name == "Document")
                {
                    var docNode = topnode.FirstChild;

                    foreach (XmlNode n in docNode.ChildNodes)
                    {
                        if (n.Name == "Placemark")
                        {
                            PlacemarkList.Add(n);
                        }
                        else
                        {
                            OtherNodeList.Add(n);
                        }
                    }
                    MessageBox.Show("KML loaded successfully.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return docNode;
                }
                return null;
            }
            catch (InvalidCastException e)
            {
                MessageBox.Show(e.Message);
                return null;
            }

            finally
            {

            }
        }


        private XmlDocument getNewTemplateDocument()
        {
            List<XmlNode> styleNodeList = new List<XmlNode>();
            string filename = Application.StartupPath + "/Files/TemplateXML.xml";
            XmlDocument xmlTemplate = new XmlDocument();
            xmlTemplate.LoadXml(XDocument.Load(filename).ToString());

            XmlDeclaration xmlDeclaration = xmlTemplate.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = xmlTemplate.DocumentElement;
            xmlTemplate.InsertBefore(xmlDeclaration, root);
            

            if (xmlTemplate.DocumentElement.Name == "kml" && xmlTemplate.DocumentElement.FirstChild.Name == "Document")
            {
                XmlNodeList childs = xmlTemplate.DocumentElement.FirstChild.ChildNodes;
                foreach (XmlNode n in childs)
                {
                    if (n.Name.Contains("Style"))
                    {
                        styleNodeList.Add(n);
                    }
                }
            }
            CopyStyleColorNodes(xmlTemplate, styleNodeList);

            //Standardfarbe setzen
            foreach (XmlNode n in styleNodeList)
            {
                SearchAndReplace_HEXCOLOR(n, "0288D1");
            }

            return xmlTemplate;
        }


        void CopyStyleColorNodes(XmlDocument xmlDocument, List<XmlNode> styleNodeList)
        {
            foreach (XmlNode n in OtherNodeList)
            {
                if (n.Name.Contains("Style"))
                {
                    string attr = n.Attributes.GetNamedItem("id").Value;
                    if (attr.Contains("placemark-"))
                    {
                        string colorname = attr.Replace("placemark-", "");
                        CopyStyleNodesToDocument(xmlDocument, styleNodeList, colorname);
                    }
                }
            }
        }

        void PlacemarkConversion(XmlNode placemark)
        {
            foreach (XmlNode n in placemark.ChildNodes)
            {
                if (n.Name.Contains("styleUrl"))
                {
                    string styleUrl = n.InnerText;
                    if (styleUrl.Contains("placemark-"))
                    {
                        string color = styleUrl.Replace("#placemark-", "");
                        n.InnerText = "#icon-1899-" + GetHexColorAsString(color);
                    }

                }
            }

        }


        private string GetHexColorAsString(string color)
        {

            switch (color)
            {
                case "deeppurple":
                    return "580479";
                case "deeporange":
                    return "B27300";
                case "bluegray":
                    return "4E6E81";
            }

            int ColorValue = Color.FromName(color).ToArgb();
            string ColorHex = string.Format("{0:x6}", ColorValue);
            if (ColorHex.Length > 6)
            {
                int start = ColorHex.Length - 6;
                ColorHex = ColorHex.Substring(start, 6);
            }
            return ColorHex;
        }

        void CopyStyleNodesToDocument(XmlDocument xmlDocument, List<XmlNode> styleNodeList, string color)
        {
            string ColorHex = GetHexColorAsString(color);

            foreach (XmlNode styleNode in styleNodeList)
            {
                XmlNode newNode = xmlDocument.ImportNode(styleNode, true);
                xmlDocument.DocumentElement.FirstChild.AppendChild(newNode);
                SearchAndReplace_HEXCOLOR(newNode, ColorHex);
            }

        }

        void SearchAndReplace_HEXCOLOR(XmlNode newNode, string ColorHex)
        {
            newNode.Attributes.GetNamedItem("id").Value = newNode.Attributes.GetNamedItem("id").Value.Replace("ReplaceWith_HEXCOLOR", ColorHex);
            newNode.InnerXml = newNode.InnerXml.Replace("<color>ffd18802</color>", "<color>ff"+ ColorHex + "</color>");
            newNode.InnerXml = newNode.InnerXml.Replace("ReplaceWith_HEXCOLOR", ColorHex);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var xmlDocument = getNewTemplateDocument();
            xmlDocument.Save(Application.StartupPath + "/Result/TEST" + ".kml");
        }

        void WriteXML(List<XmlNode> PlacemarkList, int splitinc)
        {
            int i = 0;
            int s = 0;
            int x = 0;

            var xmlDocument = getNewTemplateDocument();

            System.IO.Directory.CreateDirectory(ResultPath);

            foreach (XmlNode n in PlacemarkList)
            {
                x++;
                if (cb_country.Text != "")
                {
                    if (IsInCountry(n, cb_country.Text))
                    {
                        XmlNode newNode = xmlDocument.ImportNode(n, true);
                        PlacemarkConversion(newNode);
                        xmlDocument.DocumentElement.FirstChild.AppendChild(newNode);
                        i++;
                    }
                } else
                {
                    XmlNode newNode = xmlDocument.ImportNode(n, true);
                    PlacemarkConversion(newNode);
                    xmlDocument.DocumentElement.FirstChild.AppendChild(newNode);
                    i++;
                }

                if (i == splitinc)
                {
                    i = 0;
                    s++;
                    xmlDocument.Save(ResultPath + cb_country.Text + "Split" + s + ".kml");
                    xmlDocument = getNewTemplateDocument();
                }

                System.Windows.Forms.Application.DoEvents();
                this.Enabled = false;
                lb_wait.Text = "Items: " + Convert.ToString(x) + "/"  + Convert.ToString(i) + Environment.NewLine +  "Files: " + Convert.ToString(s);
            }
            s++;
            xmlDocument.Save(ResultPath + cb_country.Text + "Split" + s + ".kml");
            lb_wait.Text = "Items: " + Convert.ToString(x) + "/" + Convert.ToString(i) + Environment.NewLine + "Files: " + Convert.ToString(s);

            System.Windows.Forms.Application.DoEvents();
            this.Enabled = true;

            MessageBox.Show("New KML files created successfull. Check the 'Result' folder.", "Hinweis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            System.Diagnostics.Process.Start(ResultPath);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var color = textBox1.Text;
            MessageBox.Show(GetHexColorAsString(color));
        }


        private bool IsInCountry(XmlNode placemark, string country)
        {
            foreach (XmlNode n in placemark.ChildNodes)
            {
                if (n.Name.Contains("Point") | n.Name.Contains("LineString"))
                {
                    if (n.ChildNodes.Item(0).Name == "coordinates")
                    {
                        char[] separators = new char[] { ',' };
                        string[] longlat = n.ChildNodes.Item(0).InnerText.Split(separators);
                        double lng = Convert.ToDouble(longlat[0], new CultureInfo("en-US"));
                        double lat = Convert.ToDouble(longlat[1], new CultureInfo("en-US"));

                        var location = new GeoLocation { Longitude = lng, Latitude = lat };
                        var info = gServce.FindCountry(location);
                        if (info == null) { return false; }
                        return (info.Name == country);
                    }
                }
            }
            return false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (System.IO.Directory.Exists(ResultPath))
            {
                System.Diagnostics.Process.Start(ResultPath);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.frederikm.de/");
        }
    }
}

