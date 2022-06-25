using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;

using Microsoft.Extensions.Configuration;

namespace APIApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailReadController : ControllerBase
    {
        private readonly ILogger<MailReadController> _logger;

        private readonly IConfiguration _configuration;

        public MailReadController(ILogger<MailReadController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("api/ExtractEmail")]
        public IActionResult ExtractEmail(string email)
        {
            try
            {

                email = Regex.Unescape(email);

                Expense expense = new Expense();

                //Get the tax rate value from configuration 
                decimal taxrate = Convert.ToDecimal(_configuration.GetSection("Taxrate").Value);

                //Check open and close tag exists
                Match tagcheck = Regex.Match(email, "<.*>(.*)</.*>");

                if (tagcheck.Success)
                {


                    var Documents = tagcheck.Value.Split("<");
                    StringBuilder sr = new StringBuilder();
                    foreach (var doc in Documents)
                    {

                        if (doc != "")
                        {
                            if (doc.StartsWith('/'))
                            {
                                sr.Append("<" + doc.Substring(0, doc.IndexOf('>') + 1));
                            }
                            else
                            {
                                sr.Append("<" + doc);
                            }
                        }

                    }

                    string xmlDocs = "<data>" + sr.ToString() + "</data>";

                    XmlDocument xmlDoc = new XmlDocument();

                    //Check extracted data has Opening tags that have  corresponding closing tag
                    try
                    {
                        xmlDoc.LoadXml(xmlDocs);
                    }
                    catch (Exception ex)
                    {
                        //Opening tags that have no corresponding closing tag. In this case the entire message must be rejected. 
                        var message = "Opening tags that have no corresponding closing tag or Closing tags that have no corresponding Opening tag";
                        return NotFound(message);
                    }
                    Match resulttotal = Regex.Match(xmlDocs, "<total>(.*)</total>");
                    Match resultcostcentre = Regex.Match(xmlDocs, "<cost_centre>(.*)</cost_centre>");
                    Match resultpayment = Regex.Match(xmlDocs, "<payment_method>(.*)</payment_method>");
                    Match resultvendor = Regex.Match(xmlDocs, "<vendor>(.*)</vendor>");
                    Match resultdescription = Regex.Match(xmlDocs, "<description>(.*)</description>");
                    Match resultdate = Regex.Match(xmlDocs, "<date>(.*)</date>");
                    if (resulttotal.Success)
                    {

                        expense.total = Convert.ToDecimal(Convert.ToString(resulttotal.Groups[1]).Replace(",", ""));

                        //Missing<cost_centre>.In this case value of the ‘cost centre’ field in the output must default to ‘UNKNOWN
                        expense.cost_centre = Convert.ToString(resultcostcentre.Groups[1]) == "" ? "UNKNOWN" : Convert.ToString(resultcostcentre.Groups[1]);


                        expense.payment_method = Convert.ToString(resultpayment.Groups[1]);
                        expense.vendor = Convert.ToString(resultvendor.Groups[1]);
                        expense.descripion = Convert.ToString(resultdescription.Groups[1]);
                        expense.Date = Convert.ToString(resultdate.Groups[1]);

                        //Calculates the sales tax and total excluding tax based on the extracted <total> (the total includes tax)
                        decimal taxper = (taxrate / 100) + 1;
                        decimal netsalesprice = Math.Round(expense.total / taxper);
                        decimal salestax = expense.total - netsalesprice;
                        expense.salestax = salestax;
                        expense.totalexcludingTax = netsalesprice;

                        return Ok(expense);


                    }
                    else
                    {
                        //Missing <total>. In this case the entire message must be rejected. 
                        var message = "total tag not found";
                        return NotFound(message);
                    }

                }
                else
                {
                    //XML tags not found
                    var message = "XML tag not found";
                    return NotFound(message);
                }

            }
            catch(Exception e)
            {
                var message = e.Message;
                return NotFound(message);
            }

        }
    }
}
