using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HSA.InfoSys.WebCrawler
{

    using System.Net;
    using System.Text.RegularExpressions;

    class LinkScanner
    {

        private int numOfLinks;
        private string rootUrl, url, http;
        private UniqueLinkList<string> links;
        private List<int> linkIndex;


        //Constructor
        public LinkScanner(string url)
        {
            this.links = new UniqueLinkList<string>();
            this.url = url;
            if (this.url[this.url.Length - 1] == '/')
                this.url = this.url.Remove(this.url.Length - 1);
            this.rootUrl = getRootUrl(url);
            if ((this.http = getWebsiteContent(url)) != null)
            {
                this.linkIndex = new List<int>();
                this.numOfLinks = getNumberOfLinks();
                if (linksFound())
                    writeLinks();
            }
        }


        // Method requires an url and returns it's html code.
        private String getWebsiteContent(string url)
        {
            return new WebClient().DownloadString(url);
        }

        public bool scanFor(string pattern, int head)
        {

            for (int add = 0; head < (head + pattern.Length); head++)
            {
                if (http[head] == pattern[0 + add])
                {
                    if (++add == pattern.Length)
                    {
                        return true;
                    }
                }
                else return false;
            }
            return false;
        }

        private string concatLink(string linkEnd)
        {
            string link = "";
            List<string> start = new List<string>(url.Split('/'));
            List<string> end = new List<string>(linkEnd.Split('/'));
            foreach (string s in end)
            {
                if (!start.Contains(s))
                {
                    start.Add(s);
                }
            }
            foreach (string s in start)
            {
                link += (s + '/');
            }
            return link;
        }

        // Method counts number of links and marks the their position, inside the html code.
        private int getNumberOfLinks()
        {
            string pattern = "href=\"/";
            int number = 0, spLength = rootUrl.Length;
            for (int head = 0; head < http.Length; head++)
            {
                if (scanFor(rootUrl, head))
                {
                    number++; linkIndex.Add(head); head += rootUrl.Length;

                }
                if (scanFor(pattern, head))
                {
                    number++; linkIndex.Add(head + 6); head += pattern.Length;
                }

            }
            return number;
        }

        //method writes all links inside the http code into an List
        private void writeLinks()
        {
           // Regex rx = new Regex("");
            foreach (int linkPos in linkIndex)
            {
                string link = "";
                for (int head = linkPos; http[head] != '\"'; head++)
                    link += http[head];
                if (!link.Contains(rootUrl))
                    link = concatLink(link);
                if (Uri.IsWellFormedUriString(link, UriKind.RelativeOrAbsolute))
                {
                    links.Add(link);
                }
                link = "";
            }
        }

        //Simple test if further links were found
        public Boolean linksFound()
        {
            return (numOfLinks > 0) ? true : false;
        }

        //Overrides ToString method
        public override string ToString()
        {
            String s = links.Count + " Links auf der Seite: " + url + " gefunden:\n";
            int nr = 0;
            foreach (string st in links)
            {
                s += ++nr + ". Link ----> " + st + "\n";
            }
            return s;
        }

        private bool validateLink(string url)
        {
            try
            {
                getWebsiteContent(url);
                return true;
            }
            catch (WebException)
            {
                return false;
            }
        }

        private string getRootUrl(string url)
        {
            char slash = '/'; string rootUrl = "";
            for (int head = 0, count = 0; head != url.Length; head++)
            {
                if (url[head] == slash)
                {
                    if (++count == 3)
                        break;
                }
                rootUrl += url[head];
            }
            return rootUrl;
        }

        public List<string> getLinks()
        {
            return links;
        }
    }
}
