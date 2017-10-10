var ProLego = function () {
    var show = function () {
        $('#role-list').autoComplete({
            minChars: 1,
            source: function (term, suggest) {
                term = term.toLowerCase();
                $.post('/ProLego/RoleList',
                {

                }, function (output){
                    var suggestions = [];
                    for (i = 0; i < output.length; i++) {
                        if (~output[i].toLowerCase().indexOf(term)) {
                            suggestions.push(output[i]);
                        }
                    }
                    suggest(suggestions);
                });
            }
        });

        $('body').on('click', '.add_role', function () {
            var role = $('#new_role').val();
            if (role != '') {
                $.post('/',
                {
                    role: role
                }, function (output) {
                    if (output.success) {
                        //bind dropdownlist

                    }
                    else {
                        alert('Failed to add role !');
                    }
                });
            }
            else {
                alert('Please input role !');
            }
        });

        $('body').on('click', '.add_member', function () {
            var project_key = $('#pro_list').val();
            var role = $('#role_list').val();
            var members = $('#members').val();
            if (project_key == '') {
                alert('Please select project !');
                return false;
            }
            if (role == '') {
                alert('Please select role !');
                return false;
            }
            if (members == '') {
                alert('Please at least add one member !');
                return false;
            }
            $.post('/',
            {
                project_key: project_key,
                role: role,
                members: members
            }, function (output) {
                if (output.success) {
                    //update project member list
                }
                else {
                    alert('Failed to add members !');
                }
            })
        });
        $('body').on('click', '.delete', function () {
            if (!confirm('Do you really want to delete this member ?')) {
                return false;
            }
            var member = $(this).attr('data-name');
            var project_key = $(this).attr('data-prokey');
            var role = $(this).attr('data-role');
            if (member && project_key && role) {
                $.post('/',
                {
                    member: member,
                    project_key: project_key,
                    role: role
                }, function (output) {
                    if (output.success) {
                        //remove this member
                    }
                    else {
                        alert('Failed to remove this member !');
                    }
                })
            }
        });
        $('body').on('click', '.export', function () {

        });
    }

    return {
        init: function () {
            show();
        }
    };
}();