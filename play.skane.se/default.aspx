<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="play.skane.se._default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">

    <meta charset="UTF-8" />
    <title runat="server" id="metaPageTitel">Play Skåne</title>

    <link href="css/reset.min.css" rel="stylesheet" />

    <link href="css/main.min.css" rel="stylesheet" />
    <link href="css/extensions.min.css" rel="stylesheet" />

    <asp:Literal ID="fbMeta" runat="server"></asp:Literal>

    <asp:Literal ID="twMeta" runat="server"></asp:Literal>

    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script src="assets/selectivizr/selectivizr-min.js"></script>
    <script src="assets/bxslider/jquery.bxslider.min.js"></script>
    <script src="http://admin.brightcove.com/js/BrightcoveExperiences.js"></script>
    <script type="text/javascript" src="http://cdn.resultify.com/clients/play.skane.com/"></script>
    <script type="text/javascript">
        WebFontConfig = { fontdeck: { id: '25166' } };

        (function () {
            var wf = document.createElement('script');
            wf.src = ('https:' == document.location.protocol ? 'https' : 'http') +
            '://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js';
            wf.type = 'text/javascript';
            wf.async = 'true';
            var s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(wf, s);
        })();
    </script>
    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-7074594-11', 'auto');
        ga('send', 'pageview');

    </script>


</head>
<body class="front">
    <form id="form1" runat="server">

        <div id="header">

            <div id="logo">
                <a href="/">
                    <img src="graphics/logo.png" alt="logo" /></a>
            </div>
            <div id="remote">
                <input class="typeahead" id="search" type="text" placeholder="Sök..." />
            </div>
        </div>

        <div class="line"></div>

        <div class="top-video" id="videoContainer" runat="server"></div>
        <div class="related" id="relatedVideos" runat="server"></div>

        <div class="searchWrapper">
            <div class="search-video-listing">
                <div id="searchResults"></div>
            </div>
            <div style="clear:both;"></div>
        </div>
        <div id="wrapper" runat="server">

            <!-- Include listing 
    <?php include 'listing.php'; ?>
        -->

        </div>

        <!-- footer -->
        <div id="footer">
            <div class="section">
                <div class="region region-footer">
                    <div id="block-boxes-footer-quote-and-logos" class="block block-boxes block-boxes-simple">

                        <div class="content">
                            <div id="boxes-box-footer_quote_and_logos" class="boxes-box">
                                <div class="boxes-box-content">
                                    <p><em>Innovativt tänkande och spännande idéer är något vi brinner för. Skane.com hjälper dig att gå från ord till handling, oavsett om du bor i Skåne, vill flytta hit eller göra affärer här.</em></p>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="block-boxes-footer-contact-info" class="block block-boxes block-boxes-simple">

                        <h2 class="block-header">Kontakt<span class="arrow"></span></h2>

                        <div class="content">
                            <div id="boxes-box-footer_contact_info" class="boxes-box">
                                <div class="boxes-box-content">
                                    <p>
                                        Business Region Skåne<br />
                                        Region Skåne
                                    </p>
                                    <p>Dockplatsen 26 | 211 19 Malmö</p>
                                    <p>
                                        +46 40 675 30 01<br />
                                        <a href="mailto:info@skane.com">info@skane.com</a>
                                    </p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </form>
    <script src="js/typeahead.bundle.min.js"></script>
    <script src="js/handlebars.min.js"></script>
    <script src="js/startupScript.js"></script>
</body>
</html>
