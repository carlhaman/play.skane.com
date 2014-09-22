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



