/***
* This script demonstrates how you can build you own custom filter widgets:
* 1. Specify widget type for column:
*       columns.Add(o => o.Customers.CompanyName)
*           .SetFilterWidgetType("CustomCompanyNameFilterWidget")
* 2. Register script with custom widget on the page:
*       <script src="@Url.Content("~/Scripts/gridmvc.customwidgets.js")" type="text/javascript"> </script>
* 3. Register your own widget in Grid.Mvc:
*       GridMvc.addFilterWidget(new CustomersFilterWidget());
*
* For more documentation see: http://gridmvc.codeplex.com/documentation
*/

/***
* StatusesFilterWidget - Provides filter user interface for customer name column in this project
* This widget onRenders select list with avaliable statuses.
*/

function StatusesFilterWidget() {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["CustomStatusFilterWidget"];
    };
    /***
    * This method invokes when filter widget was shown on the page
    */
    this.onShow = function () {
        /* Place your on show logic here */
    };

    this.showClearFilterButton = function () {
        return true;
    };
    /***
    * This method will invoke when user was clicked on filter button.
    * container - html element, which must contain widget layout;
    * lang - current language settings;
    * typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    * values - current filter values. Array of objects [{filterValue: '', filterType:'1'}];
    * cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    * data - widget data passed from the server
    */
    this.onRender = function (container, lang, typeName, values, cb, data) {
        //store parameters:
        this.cb = cb;
        this.container = container;
        this.lang = lang;

        //this filterwidget demo supports only 1 filter value for column column
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };

        this.renderWidget(); //onRender filter widget
        this.loadStatuses(); //load status' list from the server
        this.registerEvents(); //handle events
    };
    this.renderWidget = function () {
        var html = '<select style="width:250px;" class="grid-filter-type statuseslist form-control">\
                    </select>';
        this.container.append(html);
    };
    /***
    * Method loads all statuses from the server via Ajax:
    */
    this.loadStatuses = function () {
        var $this = this;
        $.post("/ServiceBoard/GetStatuses", function (data) {
            //$this.fillStatuses(data.Items);
            $this.fillStatuses(data);
        });

        //debugger;
        //var url = "/ServiceBoard/GetStatuses";
        //$.ajax({
        //    url: url,
        //    data: "{}",
        //    cache: false,
        //    type: "POST",
        //    success: function (data) {
        //    },
        //    error: function (reponse) {
        //        alert("error : " + reponse);
        //    }
        //});
    };
    /***
    * Method fill statuses select list by data
    */
    this.fillStatuses = function (items) {
        var statusesList = this.container.find(".statuseslist");
        statusesList.append('<option>Select status to filter</option>');

        for (var i = 0; i < items.length; i++) {
            //statusesList.append('<option ' + (items[i] == this.value.filterValue ? 'selected="selected"' : '') + ' value="' + items[i] + '">' + items[i] + '</option>');
            statusesList.append('<option ' + (items[i] == this.value.filterValue ? 'selected="selected"' : '') + ' value="' + items[i].Text + '">' + items[i].Text + '</option>');
        }
    };
    /***
    * Internal method that register event handlers for 'apply' button.
    */
    this.registerEvents = function () {
        //get list with statuses
        var statusesList = this.container.find(".statuseslist");
        //save current context:
        var $context = this;
        //register onclick event handler
        statusesList.change(function () {
            //invoke callback with selected filter values:
            var values = [{ filterValue: $(this).val(), filterType: 1 /* Equals */ }];
            $context.cb(values);
        });
    };
}

function PrioritiesFilterWidget() {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["CustomPriorityFilterWidget"];
    };
    /***
    * This method invokes when filter widget was shown on the page
    */
    this.onShow = function () {
        /* Place your on show logic here */
    };

    this.showClearFilterButton = function () {
        return true;
    };
    /***
    * This method will invoke when user was clicked on filter button.
    * container - html element, which must contain widget layout;
    * lang - current language settings;
    * typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    * values - current filter values. Array of objects [{filterValue: '', filterType:'1'}];
    * cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    * data - widget data passed from the server
    */
    this.onRender = function (container, lang, typeName, values, cb, data) {
        //store parameters:
        this.cb = cb;
        this.container = container;
        this.lang = lang;

        //this filterwidget demo supports only 1 filter value for column column
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };

        this.renderWidget(); //onRender filter widget
        this.loadPriorities(); //load priorities' list from the server
        this.registerEvents(); //handle events
    };
    this.renderWidget = function () {
        var html = '<select style="width:250px;" class="grid-filter-type prioritieslist form-control">\
                    </select>';
        this.container.append(html);
    };
    /***
    * Method loads all priorities from the server via Ajax:
    */
    this.loadPriorities = function () {
        var $this = this;
        $.post("/ServiceBoard/GetPriorities", function (data) {
            $this.fillPriorities(data);
        });
    };
    /***
    * Method fill priorities select list by data
    */
    this.fillPriorities = function (items) {
        var prioritieslist = this.container.find(".prioritieslist");
        prioritieslist.append('<option>Select priority to filter</option>');

        for (var i = 0; i < items.length; i++) {
            prioritieslist.append('<option ' + (items[i] == this.value.filterValue ? 'selected="selected"' : '') + ' value="' + items[i].Text + '">' + items[i].Text + '</option>');
        }
    };
    /***
    * Internal method that register event handlers for 'apply' button.
    */
    this.registerEvents = function () {
        //get list with priorities
        var prioritieslist = this.container.find(".prioritieslist");
        //save current context:
        var $context = this;
        //register onclick event handler
        prioritieslist.change(function () {
            //invoke callback with selected filter values:
            var values = [{ filterValue: $(this).val(), filterType: 1 /* Equals */ }];
            $context.cb(values);
        });
    };
}

function ServiceCategoriesFilterWidget() {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["CustomServiceCategoryFilterWidget"];
    };
    /***
    * This method invokes when filter widget was shown on the page
    */
    this.onShow = function () {
        /* Place your on show logic here */
    };

    this.showClearFilterButton = function () {
        return true;
    };
    /***
    * This method will invoke when user was clicked on filter button.
    * container - html element, which must contain widget layout;
    * lang - current language settings;
    * typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    * values - current filter values. Array of objects [{filterValue: '', filterType:'1'}];
    * cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    * data - widget data passed from the server
    */
    this.onRender = function (container, lang, typeName, values, cb, data) {
        //store parameters:
        this.cb = cb;
        this.container = container;
        this.lang = lang;

        //this filterwidget demo supports only 1 filter value for column column
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };

        this.renderWidget(); //onRender filter widget
        this.loadServiceCategories(); //load service categories' list from the server
        this.registerEvents(); //handle events
    };
    this.renderWidget = function () {
        var html = '<select style="width:250px;" class="grid-filter-type servicecategorieslist form-control">\
                    </select>';
        this.container.append(html);
    };
    /***
    * Method loads all service categories from the server via Ajax:
    */
    this.loadServiceCategories = function () {
        var $this = this;
        $.post("/ServiceBoard/GetServiceCategories", function (data) {
            $this.fillServiceCategories(data);
        });
    };
    /***
    * Method fill service categories select list by data
    */
    this.fillServiceCategories = function (items) {
        var servicecategorieslist = this.container.find(".servicecategorieslist");
        servicecategorieslist.append('<option>Select service category to filter</option>');

        for (var i = 0; i < items.length; i++) {
            servicecategorieslist.append('<option ' + (items[i] == this.value.filterValue ? 'selected="selected"' : '') + ' value="' + items[i].Text + '">' + items[i].Text + '</option>');
        }
    };
    /***
    * Internal method that register event handlers for 'apply' button.
    */
    this.registerEvents = function () {
        //get list with service categories
        var servicecategorieslist = this.container.find(".servicecategorieslist");
        //save current context:
        var $context = this;
        //register onclick event handler
        servicecategorieslist.change(function () {
            //invoke callback with selected filter values:
            var values = [{ filterValue: $(this).val(), filterType: 1 /* Equals */ }];
            $context.cb(values);
        });
    };
}

function UserStatusesFilterWidget() {
    /***
    * This method must return type of registered widget type in 'SetFilterWidgetType' method
    */
    this.getAssociatedTypes = function () {
        return ["CustomUserStatusFilterWidget"];
    };
    /***
    * This method invokes when filter widget was shown on the page
    */
    this.onShow = function () {
        /* Place your on show logic here */
    };

    this.showClearFilterButton = function () {
        return true;
    };
    /***
    * This method will invoke when user was clicked on filter button.
    * container - html element, which must contain widget layout;
    * lang - current language settings;
    * typeName - current column type (if widget assign to multipile types, see: getAssociatedTypes);
    * values - current filter values. Array of objects [{filterValue: '', filterType:'1'}];
    * cb - callback function that must invoked when user want to filter this column. Widget must pass filter type and filter value.
    * data - widget data passed from the server
    */
    this.onRender = function (container, lang, typeName, values, cb, data) {
        //store parameters:
        this.cb = cb;
        this.container = container;
        this.lang = lang;

        //this filterwidget demo supports only 1 filter value for column column
        this.value = values.length > 0 ? values[0] : { filterType: 1, filterValue: "" };

        this.renderWidget(); //onRender filter widget
        this.loadUserStatuses(); //load user status' list from the server
        this.registerEvents(); //handle events
    };
    this.renderWidget = function () {
        var html = '<select style="width:250px;" class="grid-filter-type userstatuseslist form-control">\
                    </select>';
        this.container.append(html);
    };
    /***
    * Method loads all user user statuses from the server via Ajax:
    */
    this.loadUserStatuses = function () {
        var $this = this;
        $.post("/UsersAdmin/GetUserStatuses", function (data) {
            $this.fillUserStatuses(data);
        });
    };
    /***
    * Method fill user user statuses select list by data
    */
    this.fillUserStatuses = function (items) {
        var userStatusesList = this.container.find(".userstatuseslist");
        userStatusesList.append('<option>Select status to filter</option>');

        for (var i = 0; i < items.length; i++) {
            userStatusesList.append('<option ' + (items[i] == this.value.filterValue ? 'selected="selected"' : '') + ' value="' + items[i].Value + '">' + items[i].Text + '</option>');
        }
    };
    /***
    * Internal method that register event handlers for 'apply' button.
    */
    this.registerEvents = function () {
        //get list with user statuses
        var userstatusesList = this.container.find(".userstatuseslist");
        //save current context:
        var $context = this;
        //register onclick event handler
        userstatusesList.change(function () {
            //invoke callback with selected filter values:
            var values = [{ filterValue: $(this).val(), filterType: 1 /* Equals */ }];
            $context.cb(values);
        });
    };
}