from pattern.web    import  plaintext, URL, Result,Results, SearchEngine

from pattern.en     import Sentence, Chunk, parse, pprint
from pattern.search import Pattern
from pattern.graph  import Graph, Node, Edge, export
from datetime       import datetime

import urlparse, urllib, urllib2
import xml.dom.minidom
import re
import htmlentitydefs



YAHOO_LICENSE = "Bsx0rSzV34HQ9sXprWCaAWCHCINnLFtRF_4wahO1tiVEPpFSltMdqkM1z6Xubg"

YAHOO_BOSS_V1 = "yahoo_boss_v1"
YAHOO_BOSS_V2 = "yahoo_boss_v2"

# ------webseach module -----------------------------------------------------------
#
# query = ["'risk such as'", "'risks such as'" ]
# pattern  = "NP (,) such as NP"
# websearch = websearch(YAHOO_BOSS_V1,YAHOO_LICENSE)
# websearch.search(query,pattern,250,3, DownloadType.Dynamic)  
#
#

class DownloadType:
	NoDownload = 0
	Dynamic = 1 # only downloads the url if the snippet search did no yeild any pattern search results
	Full = 2


class websearch:
	
	def __init__(self,boss_version, api_id):
		self.boss_version = boss_version
		self.api_id = api_id
			
	def search( self,queries, pattern, number_of_results, depth,download ) :
		g = Graph()
		risk_results = []
		risk_tmp = []
		risk_tmp2 = []
		engine = None
		
		if self.boss_version == YAHOO_BOSS_V1:
			engine = YahooBOSS(license= self.api_id)

		print "search started......"

		risk_tmp = self.process(engine.search(' or '.join(queries),start=1,count= number_of_results), pattern,download)
		
		if depth == 1:
			return risk_tmp 
		elif depth > 1:
			risk_results.extend(risk_tmp)
			
			for i in range(depth - 1):

				if len(risk_tmp2) > 0 :
					risk_tmp = risk_tmp2
					risk_tmp2 = [] # reset tmp

				for x,v in enumerate(risk_tmp):
					risk_tmp2.extend(self.process( engine.search(build_query(v.risky_terms) , start=1,
										count= number_of_results),pattern, download))

				risk_results.extend(risk_tmp2)

			print "search completed!!!"
			return risk_results
		else:
			return risk_tmp

	def process(self,results, pattern, download):
		risk_results = []
		body = ''
		for r in results:
			p = Pattern.fromstring(pattern)
			url = URL(r.url)
			s = Sentence(parse(r.description))
			p_search =  p.search(s)
			
			if download == DownloadType.Full or (download == DownloadType.Dynamic and len(p_search) == 0):
				try:
					#memetype and download url operation can throw 500+ & 400+, in those cases we
					#can escape the exception gracefully without halting the search
					if url.mimetype == "text/html":
						body = str(r.download(timeout=110, cached=True, proxy=None).encode("utf-8"))  
						body = plaintext(body)
				except:
					#there are cases where plaintext func fails to extract just text, we catch that exception,
					#however the body text is returned as html therefore the pattern search maybe not be 
					#reliable. Our choices are to skip this search intirely or  attemp to extract the
					#pattern. For now we'll skip that search result. 
					continue 
			
				p_search = p.search(Sentence(parse(body)))
	       
			else:
				body = ''
		
			result = Result(url=None)
			result.url = url
			result.url_content = (body or "")
			result.query= r.query
			result.sentence = s

			risky_terms = []

			for m in p_search:
				
				rightNP = ''
				for chunk in m.constituents(p[-1]): #Right NP, get all NP elements in the list
					rightNP += chunk.string + " "
				
				risky_terms.append(rightNP)
				
				"""
				leftNP = m.constituents(p[+0])[-1] # Left NP.
				leftNP = (isinstance(leftNP, Chunk) and leftNP.head or leftNP).string

				c = leftNP

				if leftNP and rightNP:
					if leftNP not in g:
						g.add_node(leftNP, radius=4, stroke=(0,0,0,0.8))
					if rightNP not in g:
						g.add_node(rightNP, radius=4, stroke=(0,0,0,0.8))
					if c not in g:
						g.add_node(c, radius=4, stroke=(0,0,0,0.8))

					g.add_edge(g[leftNP], g[c], stroke=(0,0,0,0.6))
					g.add_edge(g[c], g[rightNP], stroke=(0,0,0,0.6))
				 """
			
			if len(risky_terms) > 0 :
				result.risky_terms = risky_terms
				risk_results.append(result)

		"""
		g = g.split()[0] # Largest subgraph.

		for n in g.sorted()[:40]: # Sorted by Node.weight.
			n.fill = (0.0, 0.5, 1.0, 0.7 * n.weight)
		export(g, 'testtest', directed=True, weighted=0.6, distance=14, force=0.05, repulsion=150)
		"""

		return risk_results

# build_query(["operational, financial"]) --> "'operation risk such as' or 'finacial risk such as'"
def build_query(s):
	return ' or '.join(map(lambda x: '+"' + x.rstrip() + ' risk such as"', eval(s)))

def encode_utf8(string):
    """ Returns the given string as a Python byte string (if possible).
    """
    if isinstance(string, unicode):
        try: 
            return string.encode("utf-8")
        except:
            return string
    return str(string)

def decode_entities(string):
    # From: http://snippets.dzone.com/posts/show/4569
    def _replace_entity(match):
        entity = match.group(3)
        if match.group(1) == "#"\
        or entity.isdigit(): # Catch &39; and &160; where we'd expect &#39; and &#160;
            if match.group(2) == '' : return unichr(int(entity))
            if match.group(2) == 'x': return unichr(int('0x'+entity, 16))
        else:
            cp = htmlentitydefs.name2codepoint.get(entity)
            return cp and unichr(cp) or match.group()
    if isinstance(string, basestring):
        return RE_UNICODE.subn(_replace_entity, string)[0]
    else:
        return string

RE_UNICODE   = re.compile(r'&(#?)(x?)(\w+);') # &#201;
RE_AMPERSAND = re.compile(r"\&(?!\#)")        # & not followed by #




#--- YAHOO BOSS V1-------------------------------------------------------------------------------------------

YAHOO_BOSS_V1 = "http://boss.yahooapis.com"
SEARCH = "search"
IMAGE = "image"
RELEVANCY = "relevency"
GET = "GET"


class YahooBOSS(SearchEngine):

    def __init__(self, license=None, throttle=0.2):
        SearchEngine.__init__(self, license or YAHOO_LICENSE, throttle)
        self.format = lambda x: decode_entities(x) # < > & are encoded in XML input.

    def search(self, query, type=SEARCH, start=1, count=10, sort=RELEVANCY, size=None, cached=True):

        url = YAHOO_BOSS_V1
	if   type == SEARCH : url += "/ysearch/web/v1/" + urllib.quote(str(query.encode('utf-8'))) + "?"
	else:
	    raise SearchEngineTypeError  

	n = 1
	if count > 50: n = (count/50) + 1

	query_args={
		"appid" : self.license or "",
		"style" : "raw",
		"start" : 1 + (start-1) * count,
		"abstract" : "long",
		"format"  : "xml",
		"type"    : "text,html,-pdf,-msoffice",
		"count" : count
	      }

	for k,v in query_args.items():
		url += "&" + str(k) + "=" + str(v)
    
	results = Results(YAHOO_BOSS_V1, query, type)
	results.total = count
	for i in range(n) :
		try: 
		   request = urllib2.Request(str(url))
		   respond = urllib2.urlopen(request)	
		   data = respond.read()
		except 403:
		    raise SearchEngineLimitError
              
		data = xml.dom.minidom.parseString(encode_utf8(data))
		data = data.childNodes[0]
	
		for x in data.getElementsByTagName("result"):
		    r = Result(url=None)
		    r.url         = self.format(self._parse(x, "url"))
		    r.title       = self.format(self._parse(x, "title"))
		    r.description = self.format(self._parse(x, "abstract"))
		    r.date        = self.format(self._parse(x, "date"))
		    r.query       = str(query.encode('utf-8'))
		    results.append(r)
		
		nextpage = data.getElementsByTagName("nextpage")
		#grab the next search results url
		if len(nextpage) > 0 and len(nextpage[0].childNodes) > 0: 
			url = data.getElementsByTagName("nextpage")[0].childNodes[0].nodeValue
			url = YAHOO_BOSS_V1 + url
		else:
			break 


        return results
            
    def _parse(self, element, tag):
        # Returns the value of the first child with the given XML tag name (or None).
        tags = element.getElementsByTagName(tag)
        if len(tags) > 0 and len(tags[0].childNodes) > 0:
           # assert tags[0].childNodes[0].nodeType == xml.dom.minidom.Element.TEXT_NODE
            return tags[0].childNodes[0].nodeValue

#----YAHOO BOSS V2 -------------------------------------------------------------------------------------
"""
started coding  for yahoo boss V2, but this is far from finished


YAHOO_BOSS_V2 = "http://yboss.yahooapis.com/ysearch/"
OAUTH_CONSUMER_KEY = "dj0yJmk9a1Uzb21tR0hNQ2VVJmQ9WVdrOVVEbHFXazl6TTJVbWNHbzlNVEU0TXpRNE5URTJNZy0tJnM9Y29uc3VtZXJzZWNyZXQmeD00ZA--"
OAUTH_CONSUMER_SECRET = "b31ee512a64e0c6ccb70e522a72e1ae722f14f5a"

class YahooBOSSV2(SearchEngine):

    def get_outh_url():

	params = {
	    'oauth_version': "1.0",
	    'oauth_nonce': oauth.generate_nonce(),
	    'oauth_timestamp': oauth.generate_timestamp()
	}
	# Set up instances of our Token and Consumer. The Consumer.key and 
	# Consumer.secret are given to you by the API provider. The Token.key and.
	consumer = oauth.Consumer(key=OAUTH_CONSUMER_KEY , secret=OAUTH_CONSUMER_SECRET)

	# Set our token/key parameters
	params['oauth_consumer_key'] = consumer.key

	# Create our request. Change method, etc. accordingly.
	req = oauth.Request(method="GET", url=YAHOO_BOSS, parameters=params)

	# Sign the request.
	signature_method = oauth.SignatureMethod_HMAC_SHA1()
	req.sign_request(signature_method, consumer,None )

	return req.to_url()
	
    def __init__(self, license=None, throttle=0.1):
        SearchEngine.__init__(self, license or YAHOO_LICENSE, throttle)
        self.format = lambda x: decode_entities(x) # < > & are encoded in XML input.

    def search(self, query, type=SEARCH, start=1, count=10, sort=RELEVANCY, size=None, cached=True, **kwargs):

        url = get_outh_url()

        if   type == SEARCH : url += "web?"
	elif type == IMAGE  : url += "image?"
        elif type == NEWS   : url += "news?"
        else:
            raise SearchEngineTypeError
        if not query or count < 1 or start > 1000/count: 
            return Results(YAHOO_BOSS_V2, query, type)

        url = URL(url, method=GET, query={
              "query" : query
        })

        kwargs.setdefault("throttle", self.throttle)

        try: 
            data = url.download(cached=cached, **kwargs)
        except HTTP403Forbidden:
            raise SearchEngineLimitError
        data = xml.dom.minidom.parseString(bytestring(data))
        data = data.childNodes[0]
        results = Results(YAHOO, query, type)
        results.total = data.attributes.get("totalResultsAvailable")
        results.total = results.total and int(results.total.value) or None
        for x in data.getElementsByTagName("Result"):
            r = Result(url=None)
            r.url         = self.format(self._parse(x, "Url"))
            r.title       = self.format(self._parse(x, "Title"))
            r.description = self.format(self._parse(x, "Summary"))
            r.date        = self.format(self._parse(x, "ModificationDate"))
            r.author      = self.format(self._parse(x, "Publisher"))
            r.language    = self.format(self._parse(x, "Language"))
            results.append(r)
        return results
            
    def _parse(self, element, tag):
        # Returns the value of the first child with the given XML tag name (or None).
        tags = element.getElementsByTagName(tag)
        if len(tags) > 0 and len(tags[0].childNodes) > 0:
            assert tags[0].childNodes[0].nodeType == xml.dom.minidom.Element.TEXT_NODE
            return tags[0].childNodes[0].nodeValue


"""

query = ["'financial risk such as'", "'market risks such as'" ]
pattern  = "NP (,) such as NP" 

websearch = websearch(YAHOO_BOSS_V1,YAHOO_LICENSE)
results = websearch.search(query,pattern,100,3, DownloadType.Dynamic)  


f = open('taxonomy', 'w')

for k,v in enumerate(results):
	f.write( ''.join(v.risky_terms) + "\r\n")

