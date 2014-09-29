function showCategory(categoryId) {
    $('.video-listing').html($(categoryId).html());
    var minHeight = $('.video-listing').height() + 25;

    if (minHeight <= 300) { $('.categories').height(300); }
    else { $('.categories').height(minHeight);}
    
    infoBoxShow();
}
function infoBoxShow() {
    $(function () {
        $('.post').hover(function () {
            var boxHeight = ($(this).find('div.info-box').height() + 26);
            var test = 0 - boxHeight;

            $(this).find('div.info-box').css('top', test);
            $(this).find('div.info-box').show();
        },
        function () {
            $('div.info-box').hide();
        });
    });
}
(function ($) {

    $(document).ready(function () {

        //kod från Oddhill
        // Only create the slider if there are more than one slide available.
        if ($('.related-videos .slider-videos').children().length > 4) {
            // Create the slider.
            $('.related-videos .slider-videos').bxSlider({
                minSlides: 4,
                maxSlides: 50,
                moveSlides: 1,
                slideWidth: 200,
                pager: false,
            });
        }
        $('.categories > ul > li > a')[0].click();
        $('.categories').height($('.video-listing').height() + 25);

    });

    $('div.info-box').hide();
   
    infoBoxShow();



})(jQuery);


var videoSearch = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.obj.whitespace('title'),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: '../search.aspx?query=%QUERY'
});

videoSearch.initialize();

$('#remote .typeahead').typeahead(null, {
    highlight: true,
    name: 'search-results',
    displayKey: 'title',
    source: videoSearch.ttAdapter(),
    templates: {
        empty: [
          '<p class="empty-message">',
          'No videos match the current query',
          '</p>'
        ].join('\n'),
        suggestion: Handlebars.compile('<a href="default.aspx?bctid={{bcid}}"><p><strong>{{title}}</strong></p></a>')
    }
});

$('#search').keypress(function (e) {
    if (e.which == 13) {//Enter key pressed
        var query = $('#search').val();

        if (query.length >= 1) {
            $.ajax({
                url: "../search.aspx?query=" + query,
                dataType: "json"
            }).success(function (data) {
                $('#searchResults').empty();
                $('#searchResults').append("<h2>Sökresultat för " + query + "</h2>");
                $.each(data, function (i, item) {
                    $('#searchResults').append(
                        "<div class=\"post\">"
                        + "<a href=\"?bctid=" + data[i].bcid + " \" class=\"videoBox\">"
                            + "<div class=\"info-box\" style=\"top: -416px; display: none;\">"
                                + "<h2>" + data[i].title + "</h2>"
                               + "<img src=\"" + data[i].imageURL + "\"/><p>" + data[i].shortDescription + "</p>"
                            + "</div>"
                            + "<div class=\"item-video\">"
                               + "<img src=\"" + data[i].imageURL + "\" alt=\"" + data[i].shortDescription + "\"/>"
                            + "</div>"
                            + " <h3>" + data[i].title + "</h3>"
                            + "</a>"
                         + "</div>"
                        );
                })
            }).complete(function () { infoBoxShow(); });

        }
        $('html,body').animate({
            scrollTop: $("#searchResults").offset().top
        });

        
    }
});