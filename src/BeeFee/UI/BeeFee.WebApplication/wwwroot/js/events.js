var Events = {
    loadEvents: function (text, startDate, endDate, city, types, categories, pageIndex, pageSize, maxPrice) {
        var d = $.Deferred();
        var params = [];
        params.push("Text=" + text);
        params.push("StartDate=" + startDate);
        params.push("EndDate=" + endDate);
        params.push("City=" + city);
        params.push("PageIndex=" + pageIndex);
        params.push("PageSize=" + pageSize);
        params.push("MaxPrice=" + maxPrice);
        $.each(types, function (i, type) { params.push("Types=" + type); });
        $.each(categories, function (i, category) { params.push("Categories=" + category); });
        $.ajax({
            url: '/event/loadevents?' + params.join("&"),
            //data:
            //{
            //    Text: text,
            //    StartDate: startDate,
            //    EndDate: endDate,
            //    City: city,
            //    "Types[]": types,
            //    Categories: categories,
            //    PageIndex: pageIndex,
            //    PageSize: pageSize,
            //    MaxPrice: maxPrice
            //},
            cache: false,
            success: function (data) {
                d.resolve({ allLoaded: data.allLoaded, html: data.html });
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