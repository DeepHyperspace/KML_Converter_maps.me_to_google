# KML-Converter for maps.me to google kml-files
This tool can help migrate maps.me KML-files to google (maps/earth). 
If you want to migrate a large Map with over 2000 placemarks to google my maps,
you might get an error that the import is limited to 2000 placemarks. 
Another problem is, that the symbols and colors of KML-files from Maps.me are not transferred to Google Maps / Earth. I was able to solve these problems.

## Why?
https://maps.me/ recently released an update for their smartphone app that shot up the map styles for many users. A friend asked me for help, he tried to import a backup KML file to google, he exported from maps.me, but all his stylings where lost. So I wrote this program for him that can edit and migrate those maps.me KML-files. Maybe someone else has a use for it.

## Tipp
In case you didn't know, KMZ files are simply zipped KML files, so you can extract a KML file from each KMZ.

## Usage
Download the release here: https://github.com/DeepHyperspace/KML_Converter_maps.me_to_google/releases

1. First open the KML file you want to convert, by clicking the open KML file button
2. Select the number of placemarks per KML file and optionally a country, if you want to use this filter, otherwise just leave it blank.
3. Then click on the convert and save button to start the conversion.
4. You can then easily import the new KML files into Google Maps. Google Earth works too.

![Image of KML-Converter](https://www.frederikm.de/wp-content/uploads/2020/12/MapsPort.png)

## Please note:
Since I used Windows Forms, this application only runs on Windows.
You can only convert KML files that were created from maps.me.
If you also want to work with KML files from other programs, I have to add additional code. 
Let me know if this is necessary.

## Background Information
The basic problem is, that maps.me kml-files use a stylemark like this:
```
  <Style id="placemark-red">
    <IconStyle>
      <Icon>
        <href>http://maps.me/placemarks/placemark-red.png</href>
      </Icon>
    </IconStyle>
  </Style>
```

Google uses something like this.
The color google my maps uses are located in the style id (icon-1899-HEXCOLOR-normal)
The color that google earth uses are located in the colortag (<color>ff0288D1</color>)
```
    <Style id="icon-1899-0288D1-normal">
      <IconStyle>
        <color>ff0288D1</color>
        <scale>1</scale>
        <Icon>
          <href>https://www.gstatic.com/mapspro/images/stock/503-wht-blank_maps.png</href>
        </Icon>
        <hotSpot x="32" xunits="pixels" y="64" yunits="insetPixels" />
      </IconStyle>
      <LabelStyle>
        <scale>0</scale>
      </LabelStyle>
    </Style>
```
So i wrote an interpreter that checks the colorname like "red", and calculates the hexcode and adds it to the placemarks and styles.

Another thing i did, was that i used InquisitorJax tool to filter the Placemarks by country.
Imagine you have a Map with 30.000 placemarks all over the world. So with his tool, you can interpret geocordinates like <coordinates>6.9545449,48.283129</coordinates>
and get the country of that placemark. So you can create country specific KML-files. Currently supported placemark types are points and lines.

## Notes
This was just a quick and dirty project i did in some hours, please dont expect high quality standards ;-)
Nevertheless you can contact me in case of errors.

## Packages i used for this project
Contribs go to: https://github.com/InquisitorJax/Wibci.CountryReverseGeocode InquisitorJax for his awesome tool-port.

Get it from nuget here: https://www.nuget.org/packages/Wibci.CountryReverseGeocode/

