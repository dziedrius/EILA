**EILA (Embeddable IIS Log Analyser)** idea is simple:

You have a site, running on IIS and you need some basic statistics 
for monitoring how your server is used.

EILA consists of several parts:    
**Eila.Analyser**     
That is a console application, that analyses log files and produces charts and csv files with IIS usage data.
Normally you schedule Eila.Analyser to run once a day and by default it processes yesterday's logs.
Analyser uses Microsoft's LogParser library to query logs, so it is very fast and you can write very flexible queries.

     Usage: Eila.Analyser [path] [daysBeforeToday]
       [path] - path where to store processed charts and csv files
       [daysBeforeToday] - logs will be processed of a day, that is [daysBeforeToday],
                           for example 1 means yesterday. If none supplied, yesterday will be used.

**Eila.HttpHandler**     
That is a HttpModule, that knows how to read processed logs directory and render it as a web page, for easy analysis browsing.
You can install this HttpModule using nuget:

     Install-Package Eila

**Eila.Queries**     
Is an assembly, where IIS log queries are kept. You can add own queries, Eila.Analyser will catch them from Eila.Queries.dll during runtime.
Right now there are these queries:    
* HitsPerMinute - how many hits site is receiving per minute.    
* HitsPerHour - how many hits site is receiving per hour.    
* ResponseTimeQuery - how reqest response time is distributed.   
* ResponseTimesByUrl - top 25 urls, that had longest response times.   
* Top25FileTypes - what file types were requested most.   
* Top25Urls - what urls were requested most.
