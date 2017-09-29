var Schedule = function () {
    var show = function (container, id) {
        moment().zone('-08:00');

        var CALENDAR = $('#' + container);
        var jo_num = $("#" + id).val();

        var $addEventModal = $('#addEventModal');
        $addEventModal.appendTo('body');
        $addEventModal.on('show.bs.modal', function () {
            $addEventModal.find('form')[0].reset();
        });
        $addEventModal.on('hide.bs.modal', function () {
            eventToUpdate = null;
        });

        var startDate, endDate, eventToUpdate;

        var $input = $addEventModal.find('[name=date-period]');

        $input.daterangepicker({
            "timePicker": true,
            "timePicker12Hour": false,
            "timePickerIncrement": 10,
            "format": 'YYYY-MM-DD'
        }, function (start, end) {
            startDate = start;
            endDate = end;
        });

        $addEventModal.find('[name=title]').change(function () {
            var $check = $(this);
            var stDate = $input.data('daterangepicker').startDate;
            if (!eventToUpdate || !eventToUpdate.end) {
                $input.data('daterangepicker').setStartDate(stDate);
                var enDate = stDate.add(2, 'hours');
                $input.data('daterangepicker').setEndDate(enDate);
            } else if (eventToUpdate) {
                $input.data('daterangepicker').setStartDate(eventToUpdate.start);
                if (eventToUpdate.end) {
                    $input.data('daterangepicker').setEndDate(eventToUpdate.end);
                }
            }
        });

        $addEventModal.find('.btn-add').click(function () {
            var $body = $(this).closest('.modal-content');
            var style = $body.find('[name=style]:checked').val();
            var title = $body.find('[name=title]').val();
            var desc = $body.find('[name=desc]').val();

            var startDate = $input.data('daterangepicker').startDate;
            var endDate = $input.data('daterangepicker').endDate;
            if (!eventToUpdate) {

                var event = {
                    title: title,
                    className: [style, style.replace('bg', 'border') + '-dark'],
                    description: desc
                };

                event.start = startDate;
                event.end = endDate;

                var myevent = {
                    jonum: jo_num,
                    title: title,
                    className: [style, style.replace('bg', 'border') + '-dark'],
                    description: desc,
                    start: startDate.format('YYYY-MM-DD HH:mm:ss'),
                    end: endDate.format('YYYY-MM-DD HH:mm:ss')
                };
                $.ajax('/BRTrace/AddScheduleEvent', {
                    data: myevent,
                    method: 'POST',
                    cache: false
                }).done(function (res) {
                    if (res.success) {
                        event.id = res.id;
                        CALENDAR.fullCalendar('renderEvent', event, true);
                    } else {

                    }
                });

            } else {

                eventToUpdate.title = title;
                eventToUpdate.className = [style, style.replace('bg', 'border') + '-dark'];
                eventToUpdate.description = desc;
                eventToUpdate.start = startDate;
                eventToUpdate.end = endDate;

                CALENDAR.fullCalendar('updateEvent', eventToUpdate);

                var myevent = {
                    jonum: jo_num,
                    id: eventToUpdate.id,
                    title: title,
                    className: [style, style.replace('bg', 'border') + '-dark'],
                    description: desc,
                    start: startDate.format('YYYY-MM-DD HH:mm:ss'),
                    end: endDate.format('YYYY-MM-DD HH:mm:ss')
                };

                $.ajax('/BRTrace/UpdateScheduleEvent', {
                    data: myevent,
                    method: 'POST',
                    cache: false
                }).done(function (res) {
                    if (res.success) {

                    } else {

                    }
                });
            }

            $addEventModal.modal('hide');
        });

        $addEventModal.find('.btn-danger').click(function () {

            if (eventToUpdate) {

                var myevent = {
                    id: eventToUpdate.id
                }

                $.ajax('/BRTrace/RemoveScheduleEvent', {
                    data: myevent,
                    method: 'POST',
                    cache: false
                }).done(function (res) {
                    if (res.success) {
                        CALENDAR.fullCalendar('removeEvents', myevent.id);
                    } else {

                    }
                });
            }
            $addEventModal.modal('hide');
        });

        var myDate = new Date();
        var mydatestr = myDate.getUTCFullYear() + '-' + ("0" + (myDate.getUTCMonth() + 1)).slice(-2) + '-' + ("0" + myDate.getUTCDate()).slice(-2);

        CALENDAR.fullCalendar({
            dayClick: function (date, jsEvent, view) {
                $addEventModal.modal('show');
                $input.data('daterangepicker').setOptions({
                    "timePicker": true,
                    "timePicker12Hour": false,
                    "timePickerIncrement": 10,
                    "format": 'YYYY-MM-DD'
                });
                $addEventModal.find('[name=title]')[0].selectedIndex = 0;
                $addEventModal.find('.btn-add').html('Add event');
                $addEventModal.find('.btn-danger').hide();
                $addEventModal.find('[name=date-period]').data('daterangepicker').setStartDate(date);
                $addEventModal.find('[name=date-period]').data('daterangepicker').setEndDate(date);
            },
            header: {
                left: 'today',
                center: 'title',
                right: 'prev,next'
            },
            defaultDate: mydatestr,
            editable: true,
            droppable: true,
            eventLimit: true,
            businessHours: true,
            buttons: false,
            eventRender: function (event, element, view) {
                //if (event.description) {
                //    element.append('<span class="fc-description">' + event.description + '</span>');
                //}
            },
            eventClick: function (calEvent, jsEvent, view) {
                eventToUpdate = calEvent;
                $addEventModal.modal('show');
                $addEventModal.find('.btn-add').html('Update event');
                $addEventModal.find('.btn-danger').show();
                var className = calEvent.className;
                if (className && typeof className === 'string') {
                    $addEventModal.find('[name="style"][value="' + className + '"]').prop('checked', true);
                } else if (className && className.length > 0) {
                    for (var i = 0; i < className.length; i++) {
                        if (className[i].indexOf('bg') === 0) {
                            $addEventModal.find('[name="style"][value="' + className[i] + '"]').prop('checked', true);
                            break;
                        }
                    }
                }

                if (!calEvent.end) {
                    calEvent.end = moment();
                }

                $addEventModal.find('[name=date-period]').data('daterangepicker').setStartDate(calEvent.start);
                $addEventModal.find('[name=date-period]').data('daterangepicker').setEndDate(calEvent.end);

                $addEventModal.find('[name=title]').val(calEvent.title);
                $addEventModal.find('[name=desc]').val(calEvent.description || '');
                CALENDAR.fullCalendar('updateEvent', calEvent);

                // change the border color just for fun
                $(this).css('border-color', 'red');

            },
            eventDrop: function (calEvent, jsEvent, view) {
                var myevent = {
                    id: calEvent.id,
                    start: calEvent.start.format('YYYY-MM-DD HH:mm:ss'),
                    end: calEvent.end.format('YYYY-MM-DD HH:mm:ss')
                };

                $.ajax('/BRTrace/MoveScheduleEvent', {
                    data: myevent,
                    method: 'POST',
                    cache: false
                });
            },
            eventResize: function (calEvent, jsEvent, view) {
                var myevent = {
                    id: calEvent.id,
                    start: calEvent.start.format('YYYY-MM-DD HH:mm:ss'),
                    end: calEvent.end.format('YYYY-MM-DD HH:mm:ss')
                };

                $.ajax('/BRTrace/MoveScheduleEvent', {
                    data: myevent,
                    method: 'POST',
                    cache: false
                });
            },
            events: {
                url: '/BRTrace/JOSchedules',
                type: 'POST',
                data: {
                    JoNum: jo_num
                },
                cache: false
            }

        });
    }

    return {
        init: function (container, jo_num) {
            show(container, jo_num);
        }
    };
}();