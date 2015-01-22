/*

	A really simple SVG cleaner for the CodeProject
 
    This is a *big* thanks to Chris Maunder and the team for their great work 

    Credits: Based upon a PHP implementation https://github.com/alister-/SVG-Sanitizer
 * 
	Copyright (c) Jerry Evans, 2015
	All rights reserved.

	The MIT License (MIT)

	Copyright (c) 2015 Jerry Evans

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in
	all copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
	THE SOFTWARE.

*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace svgcheck
{
    class SVGChecker
    {
        /// <summary>
        /// dictionary of string to hashset maps valid SVG tags to valid attributes. All else is discarded (!)
        /// </summary>
        static Dictionary<String, HashSet<String>> dict = new Dictionary<String, HashSet<String>>()
        {
       		{ "a", new HashSet<String>{"class", "clip-path", "clip-rule", "fill", "fill-opacity", "fill-rule", "filter", "id", "mask", "opacity", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform", "href", "xlink:href", "xlink:title"} },
		    { "circle", new HashSet<String>{"class", "clip-path", "clip-rule", "cx", "cy", "fill", "fill-opacity", "fill-rule", "filter", "id", "mask", "opacity", "r", "requiredFeatures", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform"} },
		    { "clipPath", new HashSet<String>{"class", "clipPathUnits", "id"} },
		    { "defs", new HashSet<String>{} },
	        { "style" , new HashSet<String>{"type"} },
		    { "desc", new HashSet<String>{} },
		    { "ellipse", new HashSet<String>{"class", "clip-path", "clip-rule", "cx", "cy", "fill", "fill-opacity", "fill-rule", "filter", "id", "mask", "opacity", "requiredFeatures", "rx", "ry", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform"} },
		    { "feGaussianBlur", new HashSet<String>{"class", "color-interpolation-filters", "id", "requiredFeatures", "stdDeviation"} },
		    { "filter", new HashSet<String>{"class", "color-interpolation-filters", "filterRes", "filterUnits", "height", "id", "primitiveUnits", "requiredFeatures", "width", "x", "xlink:href", "y"} },
		    { "foreignObject", new HashSet<String>{"class", "font-size", "height", "id", "opacity", "requiredFeatures", "style", "transform", "width", "x", "y"} },
		    { "g", new HashSet<String>{"class", "clip-path", "clip-rule", "id", "display", "fill", "fill-opacity", "fill-rule", "filter", "mask", "opacity", "requiredFeatures", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform", "font-family", "font-size", "font-style", "font-weight", "text-anchor"} },
		    { "image", new HashSet<String>{"class", "clip-path", "clip-rule", "filter", "height", "id", "mask", "opacity", "requiredFeatures", "style", "systemLanguage", "transform", "width", "x", "xlink:href", "xlink:title", "y"} },
		    { "line", new HashSet<String>{"class", "clip-path", "clip-rule", "fill", "fill-opacity", "fill-rule", "filter", "id", "marker-end", "marker-mid", "marker-start", "mask", "opacity", "requiredFeatures", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform", "x1", "x2", "y1", "y2"} },
		    { "linearGradient", new HashSet<String>{"class", "id", "gradientTransform", "gradientUnits", "requiredFeatures", "spreadMethod", "systemLanguage", "x1", "x2", "xlink:href", "y1", "y2"} },
		    { "marker", new HashSet<String>{"id", "class", "markerHeight", "markerUnits", "markerWidth", "orient", "preserveAspectRatio", "refX", "refY", "systemLanguage", "viewBox"} },
		    { "mask", new HashSet<String>{"class", "height", "id", "maskContentUnits", "maskUnits", "width", "x", "y"} },
		    { "metadata", new HashSet<String>{"class", "id"} },
		    { "path", new HashSet<String>{"class", "clip-path", "clip-rule", "d", "fill", "fill-opacity", "fill-rule", "filter", "id", "marker-end", "marker-mid", "marker-start", "mask", "opacity", "requiredFeatures", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform"} },
		    { "pattern", new HashSet<String>{"class", "height", "id", "patternContentUnits", "patternTransform", "patternUnits", "requiredFeatures", "style", "systemLanguage", "viewBox", "width", "x", "xlink:href", "y"} },
		    { "polygon", new HashSet<String>{"class", "clip-path", "clip-rule", "id", "fill", "fill-opacity", "fill-rule", "filter", "id", "class", "marker-end", "marker-mid", "marker-start", "mask", "opacity", "points", "requiredFeatures", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform"} },
		    { "polyline", new HashSet<String>{"class", "clip-path", "clip-rule", "id", "fill", "fill-opacity", "fill-rule", "filter", "marker-end", "marker-mid", "marker-start", "mask", "opacity", "points", "requiredFeatures", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform"} },
		    { "radialGradient", new HashSet<String>{"class", "cx", "cy", "fx", "fy", "gradientTransform", "gradientUnits", "id", "r", "requiredFeatures", "spreadMethod", "systemLanguage", "xlink:href"} },
		    { "rect", new HashSet<String>{"class", "clip-path", "clip-rule", "fill", "fill-opacity", "fill-rule", "filter", "height", "id", "mask", "opacity", "requiredFeatures", "rx", "ry", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform", "width", "x", "y"} },
		    { "stop", new HashSet<String>{"class", "id", "offset", "requiredFeatures", "stop-color", "stop-opacity", "style", "systemLanguage"} },
		    { "svg", new HashSet<String>{"class", "clip-path", "clip-rule", "filter", "id", "height", "mask", "preserveAspectRatio", "requiredFeatures", "style", "systemLanguage", "viewBox", "width", "x", "xmlns", "xmlns:se", "xmlns:xlink", "y"} },
		    { "switch", new HashSet<String>{"class", "id", "requiredFeatures", "systemLanguage"} },
		    { "symbol", new HashSet<String>{"class", "fill", "fill-opacity", "fill-rule", "filter", "font-family", "font-size", "font-style", "font-weight", "id", "opacity", "preserveAspectRatio", "requiredFeatures", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "transform", "viewBox"} },
		    { "text", new HashSet<String>{"class", "clip-path", "clip-rule", "fill", "fill-opacity", "fill-rule", "filter", "font-family", "font-size", "font-style", "font-weight", "id", "mask", "opacity", "requiredFeatures", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "text-anchor", "transform", "x", "xml:space", "y"} },
		    { "textPath", new HashSet<String>{"class", "id", "method", "requiredFeatures", "spacing", "startOffset", "style", "systemLanguage", "transform", "xlink:href"} },
		    { "title", new HashSet<String>{} },
		    { "tspan", new HashSet<String>{"class", "clip-path", "clip-rule", "dx", "dy", "fill", "fill-opacity", "fill-rule", "filter", "font-family", "font-size", "font-style", "font-weight", "id", "mask", "opacity", "requiredFeatures", "rotate", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "systemLanguage", "text-anchor", "textLength", "transform", "x", "xml:space", "y"} },
		    { "use", new HashSet<String>{"class", "clip-path", "clip-rule", "fill", "fill-opacity", "fill-rule", "filter", "height", "id", "mask", "stroke", "stroke-dasharray", "stroke-dashoffset", "stroke-linecap", "stroke-linejoin", "stroke-miterlimit", "stroke-opacity", "stroke-width", "style", "transform", "width", "x", "xlink:href", "y"} },
        };
        
        /// fully qualified path to SVG file
        /// writes with a .cleaned.svg filetype
        public static bool Check(string url)
        {
            bool ret = false;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(url);
                //
                Console.WriteLine(String.Format("Checking {0}", url));
                // get the node list
                XmlNodeList nodes = doc.GetElementsByTagName("*");
                // iterate backwards so we can pull anything suspect
                for (int node_index = nodes.Count - 1; node_index >= 0; node_index--)
                {
                    // get the node
                    XmlNode node = nodes[node_index];
                    // check tag is in our whitelist dictionary
                    String tag = node.Name;
                    // we'll want the hashed set of legitimate SVG attributes 
                    // for the next round ...
                    HashSet<String> hset = null;
                    // if ofund we are good else delete the node from the DOM
                    if (dict.TryGetValue(tag,out hset))
                    {
                        // order of iteration less important over attributes ...
                        for (int attrib_index = node.Attributes.Count -1; attrib_index >= 0; attrib_index--)
                        {
                            XmlAttribute attribute = node.Attributes[attrib_index];
                            if (hset.Contains(attribute.Name) == false)
                            {
                                node.Attributes.RemoveAt(attrib_index);
                                Console.WriteLine(String.Format("\tDeleting attribute {0}", attribute.Name));
                            }
                        }
                    }
                    else
                    {
                        nodes[node_index].ParentNode.RemoveChild(node);
                        Console.WriteLine(String.Format("Deleting node {0}", tag));
                    }
                }
                //. JME audit:improve
                String newurl = url.Replace(".svg", ".cleaned.svg");
                doc.Save(newurl);
                ret = true;
            }
            catch (System.Exception ex)
            {
                ///
                Console.WriteLine(ex.Message);
            }

            return ret;
        }
    }
}
