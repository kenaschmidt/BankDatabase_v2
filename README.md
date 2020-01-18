# BankDatabase_v2

* Note: this is a backup of an incomplete implementation.

Tool to retrieve and parse data from FFIEC web service.

Background:

The Federal Financial Institutions Examination Council (FFIEC) (https://www.ffiec.gov/) collects and maintains financial metrics
from banking institutions operating under the auspices of the FDIC and other regulatory bodies.

From website: "The Council is a formal interagency body empowered to prescribe uniform principles, standards, and report forms for
the federal examination of financial institutions by the Board of Governors of the Federal Reserve System (FRB), the Federal Deposit 
Insurance Corporation (FDIC), the National Credit Union Administration (NCUA), the Office of the Comptroller of the Currency (OCC),
and the Consumer Financial Protection Bureau (CFPB) and to make recommendations to promote uniformity in the supervision of financial 
institutions."

Financial institutions are required to submit quarterly reports covering a wide range of metrics, in standardized XBRL reporting format.

The FFIEC provides a public interface for examining submitted financial reports, allowing users to retrieve raw XBRL data or preformatted
report views (CALL reports).  This data can be used to examine individual bank behaviors, balance sheet trends, risk tendancies, performance, 
and other metrics which may be usedful sales targeting.

Purpose:

I built this tool in my previous position to help analyze banking trends in the Municipal bond market.  Banks account for a large portion
of the Muni purchasing market but do not share much information in terms of their purchasing preferances.  The data provided in these reports 
can be used to build profiles of each institution for better understanding of what, where, and how much they are buying at any given time.

Implementation Notes:

In use, the tool retrieves a list of reporting banks for each available reporting period (quarterly), then retrieves and parses an XBRL
document containing all report data for that bank and period.  

XBRL is parsed using a third-party library and saved to mapped entities for database storage.  Financial values are mapped to Key/Value
type objects (there are thousands of different values for each report).

Use:

The UI provides basic functionality for loading banks and retrieving reports for save to the localDb.  Backup from work project was
completed prior to report view/filter interface.
