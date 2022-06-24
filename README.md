# ExtractData
Packages installed
  swagger
  Testing data using swagger


Scenario
As part of a new integration, we have a need to import data from text we receive via email. The data can be a 
combination of: 
• Embedded blocks or ‘islands’ of XML content 
• Marked up fields using opening and closing tags 
The following text is an example of this:
Hi Patricia, 
Please create an expense claim for the below. Relevant details are marked up as requested…
<expense><cost_centre>DEV632</cost_centre><total>35,000</total><payment_method>personal
card</payment_method></expense>
From: William Steele
Sent: Friday, 16 June 2022 10:32 AM 
To: Maria Washington
Subject: test 
Hi Maria, 
Please create a reservation for 10 at the <vendor>Seaside Steakhouse</vendor> for our
<description>development team’s project end celebration</description> on <date>27 April 
2022</date> at 7.30pm. 
Regards, 
William
Your task is to write a REST web service that: 
• Accepts a block of text as input
• Extracts the specified data
• Calculates the sales tax and total excluding tax based on the extracted <total> (the total includes tax)
• Makes the extracted and calculated data available for retrieval
Failure Conditions 
The following failure conditions must be detected and handled as follows: 
• Opening tags that have no corresponding closing tag. In this case the entire message must be rejected. 
• Missing <total>. In this case the entire message must be rejected. 
• Missing <cost_centre>. In this case value of the ‘cost centre’ field in the output must default to ‘UNKNOWN