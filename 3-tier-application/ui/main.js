// Wait for the document to be ready
$(document).ready(function () {
    let baseUrl = '';
    let users = [];
    let activeUser = {};
    // Function to get users from 'api/user' endpoint and display them in the table
    function getUsers() {
        $.get(`${baseUrl}api/user`, function (response) {
            // Clear the table body
            $('#userTableBody').empty();

            // Loop through the users and append them to the table
            users = response.data;
            users && users.length && users.forEach(function (user) {
                var row = '<tr>' +
                    '<td>' + user.id + '</td>' +
                    '<td>' + user.name + '</td>' +
                    '<td>' + user.email + '</td>' +
                    '<td>' + user.age + '</td>' +
                    '<td>' + user.address + '</td>' +
                    '<td>' +
                    '<button class="btn btn-sm btn-info editUser" data-id="' + user.id + '">Edit</button>' +
                    '<button class="btn btn-sm btn-danger deleteUser ml-2" data-id="' + user.id + '">Delete</button>' +
                    '</td>' +
                    '</tr>';
                $('#userTableBody').append(row);
            });
        });
    }

    function addEditSuccessCallback() {
        activeUser = {};
        // Clear the form fields
        $('#name').val('');
        $('#email').val('');
        $('#age').val('');
        $('#address').val('');

        // Close the modal
        $('#addEditUserModal').modal('hide');

        // Display a success message
        showMessage('User saved successfully', 'success');

        // Refresh the user table
        getUsers();
    }

    function addEditFailureCallback(res) {
        console.log(res);
        showMessage('Failed to save user', 'error');
    }

    // Function to post user data to 'api/user' endpoint
    function saveUser() {
        const name = $('#name').val();
        const email = $('#email').val();
        const age = $('#age').val();
        const address = $('#address').val();

        if (activeUser.id > 0) {
            $.ajax({
                url: `${baseUrl}api/user/${activeUser.id}`,
                type: "PUT",
                data: JSON.stringify({ id: activeUser.id, name: name, email: email, age: age, address: address }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: addEditSuccessCallback,
                error: addEditFailureCallback
            });
        } else {
            $.ajax({
                url: `${baseUrl}api/user`,
                type: "POST",
                data: JSON.stringify({ id: 0, name: name, email: email, age: age, address: address }),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: addEditSuccessCallback,
                error: addEditFailureCallback
            });
        }
    }

    // Function to open the modal for editing or adding a user
    function openUserModal() {
        const title = !activeUser.id || activeUser.id == 0 ? 'Add User' : 'Edit User';
        $('#add-edit-modal-title').text(title);

        $('#name').val(activeUser.name);
        $('#email').val(activeUser.email);
        $('#age').val(activeUser.age);
        $('#address').val(activeUser.address);

        $('#addEditUserModal').modal('show');
    }

    // Function to confirm and delete a user
    function deleteUser(id) {
        if (confirm('Are you sure you want to delete this user?')) {
            $.ajax({
                url: `${baseUrl}api/user/${id}`,
                type: 'DELETE',
                success: function () {
                    // Display a success message
                    showMessage('User deleted successfully', 'success');

                    // Refresh the user table
                    getUsers();
                },
                error: function () {
                    // Display an error message
                    showMessage('Failed to delete user', 'error');
                }
            });
        }
    }

    // Function to display messages in the messages section
    function showMessage(message, type, isModalMessage = false) {
        var messageDiv = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
            message +
            '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
            '<span aria-hidden="true">&times;</span>' +
            '</button>' +
            '</div>';

        if (isModalMessage) {
            $('#modal-messages').html(messageDiv);
        } else {

            $('#messages').html(messageDiv);
        }
    }

    // Event handlers
    $('#addUserButton').click(addUser);
    $('#saveUserButton').click(saveUser);

    function addUser() {
        activeUser = { id: 0 };
        openUserModal();
    }

    // Event delegation for dynamically created elements
    $('#userTableBody').on('click', '.editUser', function () {
        const userId = $(this).data('id');
        activeUser = users.find(u => u.id == userId);
        openUserModal();
    });

    $('#userTableBody').on('click', '.deleteUser', function () {
        var userId = $(this).data('id');
        deleteUser(userId);
    });

    
    
    async function getText(file) {
        let myObject = await fetch(file);
        let myText = await myObject.text();
        const config = JSON.parse(myText);
        baseUrl = config.API_URL;
        // Initial load of users
        getUsers();
    }
    getText('./configs/config.json');
});
