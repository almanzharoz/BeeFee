var Events = {
    loadEvents: function (text, startDate, endDate, city, loadConcept, loadExhibition, loadExcursion, categories, pageIndex, pageSize, maxPrice) {
        var d = $.Deferred();
        $.ajax({
            url: '/event/loadevents',
            data:
            {
                Text: text,
                StartDate: startDate,
                EndDate: endDate,
                City: city,
                LoadConcert: loadConcept,
                LoadExhibition: loadExhibition,
                LoadExcursion: loadExcursion,
                Categories: categories,
                PageIndex: pageIndex,
                PageSize: pageSize,
                MaxPrice: maxPrice
            },
            cache: false,
            success: function (data) {
                d.resolve({ allLoaded: allLoaded, html: data.eventsHtml });
            },
            error: function (jqXHR, textStatus) { d.reject(jqXHR, textStatus); }
        });
        return d.promise();
    }
}
//var Events = {
//    loadEvents: function (text, startDate, endDate, city, loadConcept, loadExhibition, loadExcursion, categories, pageIndex, pageSize, maxPrice) {
//        var d = $.Deferred();
//        $.ajax({
//            url: '/event/loadevents',
//            data:
//            {
//                Text: text,
//                StartDate: startDate,
//                EndDate: endDate,
//                City: city,
//                LoadConcert: loadConcept,
//                LoadExhibition: loadExhibition,
//                LoadExcursion: loadExcursion,
//                Categories: categories,
//                PageIndex: pageIndex,
//                PageSize: pageSize,
//                MaxPrice: maxPrice
//            },
//            cache: false,
//            success: function (data) {
//                d.resolve(data.events);
//            },
//            error: function (jqXHR, textStatus) { d.reject(jqXHR, textStatus); }
//        });
//        return d.promise();
//    }
//}