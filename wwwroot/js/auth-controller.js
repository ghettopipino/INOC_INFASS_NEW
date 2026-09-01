$(document).ready(function () {


    // =========================================
    // LOAD USERS
    // DYNAMIC VIEW DATA
    // =========================================

    function loadUsers() {

        $.ajax({

            url: "/Home/GetUsers",

            type: "GET",

            success: function (response) {


                if (!response.success) {

                    alert(response.message);

                    return;
                }


                let rows = "";


                if (response.data.length === 0) {

                    rows = `
                        <tr>
                            <td colspan="5"
                                class="text-center">
                                No users found.
                            </td>
                        </tr>
                    `;

                }
                else {


                    response.data.forEach(function (user) {


                        rows += `
                            <tr id="userRow-${user.id}">

                                <td>
                                    ${user.id}
                                </td>

                                <td>
                                    ${user.fullName}
                                </td>

                                <td>
                                    ${user.email}
                                </td>

                                <td>
                                    ${user.username}
                                </td>

                                <td>

                                    <button type="button"
                                            class="btn btn-warning btn-sm editUser"
                                            data-id="${user.id}">

                                        <i class="fas fa-edit"></i>

                                        Edit

                                    </button>


                                    <button type="button"
                                            class="btn btn-danger btn-sm deleteUser"
                                            data-id="${user.id}">

                                        <i class="fas fa-trash"></i>

                                        Delete

                                    </button>

                                </td>

                            </tr>
                        `;

                    });

                }


                $("#userTableBody")
                    .html(rows);

            },


            error: function (xhr) {

                console.log(xhr.responseText);

                alert(
                    "Could not load users."
                );

            }

        });

    }


    // Load automatically
    if ($("#userTableBody").length) {

        loadUsers();

    }



    // =========================================
    // OPEN ADD USER MODAL
    // =========================================

    $("#openAddUser").click(function () {


        $("#addUserForm")[0]
            .reset();


        const modal =
            new bootstrap.Modal(
                document.getElementById(
                    "addUserModal"
                )
            );


        modal.show();

    });



    // =========================================
    // INSERT USER
    // =========================================

    $("#saveUserButton").click(function () {


        const fullName =
            $("#addFullName")
                .val()
                .trim();


        const email =
            $("#addEmail")
                .val()
                .trim();


        const username =
            $("#addUsername")
                .val()
                .trim();


        const password =
            $("#addPassword")
                .val();


        if (!fullName ||
            !email ||
            !username ||
            !password) {


            alert(
                "Please complete all fields."
            );


            return;

        }


        $.ajax({

            url: "/Home/Insert",

            type: "POST",

            data: {

                FullName: fullName,

                Email: email,

                Username: username,

                Password: password

            },


            success: function (response) {


                if (response.success) {


                    alert(
                        response.message
                    );


                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "addUserModal"
                            )
                        )
                        .hide();


                    loadUsers();


                }
                else {


                    alert(
                        response.message
                    );


                }

            },


            error: function (xhr) {


                console.log(
                    xhr.responseText
                );


                alert(
                    "Server Error: " +
                    xhr.status
                );


            }

        });

    });



    // =========================================
    // OPEN EDIT MODAL
    // =========================================

    $(document).on(
        "click",
        ".editUser",
        function () {


            const id =
                $(this).data("id");


            $.ajax({

                url: "/Home/GetUser",

                type: "GET",

                data: {

                    id: id

                },


                success: function (response) {


                    if (!response.success) {


                        alert(
                            response.message
                        );


                        return;

                    }


                    const user =
                        response.data;


                    $("#editId")
                        .val(user.id);


                    $("#editFullName")
                        .val(user.fullName);


                    $("#editEmail")
                        .val(user.email);


                    $("#editUsername")
                        .val(user.username);


                    $("#editPassword")
                        .val(user.password);


                    const modal =
                        new bootstrap.Modal(
                            document.getElementById(
                                "editUserModal"
                            )
                        );


                    modal.show();

                },


                error: function (xhr) {


                    console.log(
                        xhr.responseText
                    );


                    alert(
                        "Could not load user."
                    );

                }

            });

        }
    );



    // =========================================
    // UPDATE USER
    // =========================================

    $("#updateUserButton").click(function () {


        const id =
            $("#editId")
                .val();


        const fullName =
            $("#editFullName")
                .val()
                .trim();


        const email =
            $("#editEmail")
                .val()
                .trim();


        const username =
            $("#editUsername")
                .val()
                .trim();


        const password =
            $("#editPassword")
                .val();


        if (!id ||
            !fullName ||
            !email ||
            !username ||
            !password) {


            alert(
                "Please complete all fields."
            );


            return;

        }


        $.ajax({

            url: "/Home/Update",

            type: "POST",

            data: {

                Id: id,

                FullName: fullName,

                Email: email,

                Username: username,

                Password: password

            },


            success: function (response) {


                if (response.success) {


                    alert(
                        response.message
                    );


                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "editUserModal"
                            )
                        )
                        .hide();


                    loadUsers();


                }
                else {


                    alert(
                        response.message
                    );


                }

            },


            error: function (xhr) {


                console.log(
                    xhr.responseText
                );


                alert(
                    "Server Error: " +
                    xhr.status
                );


            }

        });

    });



    // =========================================
    // DELETE USER
    // =========================================

    $(document).on(
        "click",
        ".deleteUser",
        function () {


            const id =
                $(this).data("id");


            const confirmDelete =
                confirm(
                    "Are you sure you want to delete this user?"
                );


            if (!confirmDelete) {

                return;

            }


            $.ajax({

                url: "/Home/Delete",

                type: "POST",

                data: {

                    id: id

                },


                success: function (response) {


                    if (response.success) {


                        alert(
                            response.message
                        );


                        loadUsers();


                    }
                    else {


                        alert(
                            response.message
                        );


                    }

                },


                error: function (xhr) {


                    console.log(
                        xhr.responseText
                    );


                    alert(
                        "Server Error: " +
                        xhr.status
                    );


                }

            });

        }
    );



    // =========================================
    // LOGIN
    // =========================================

    $("#loginForm").submit(function (e) {


        e.preventDefault();


        const username =
            $("#loginUsername")
                .val()
                .trim();


        const password =
            $("#loginPassword")
                .val();


        if (!username ||
            !password) {


            alert(
                "Please enter username and password."
            );


            return;

        }


        $.ajax({

            url: "/Home/login",

            type: "POST",

            data: {

                Username: username,

                Password: password

            },


            success: function (response) {


                if (response.success) {


                    alert(
                        response.message
                    );


                    window.location.href =
                        "/Home/Index";


                }
                else {


                    alert(
                        response.message
                    );


                }

            },


            error: function (xhr) {


                console.log(
                    xhr.responseText
                );


                alert(
                    "Server Error: " +
                    xhr.status
                );


            }

        });

    });



    // =========================================
    // REGISTER PAGE
    // =========================================

    $("#registerForm").submit(function (e) {


        e.preventDefault();


        const fullName =
            $("#registerFullName")
                .val()
                .trim();


        const email =
            $("#registerEmail")
                .val()
                .trim();


        const username =
            $("#registerUsername")
                .val()
                .trim();


        const password =
            $("#registerPassword")
                .val();


        const confirmPassword =
            $("#registerConfirmPassword")
                .val();


        if (!fullName ||
            !email ||
            !username ||
            !password ||
            !confirmPassword) {


            alert(
                "Please complete all fields."
            );


            return;

        }


        if (password !==
            confirmPassword) {


            alert(
                "Passwords do not match."
            );


            return;

        }


        $.ajax({

            url: "/Home/register",

            type: "POST",

            data: {

                FullName:
                    fullName,

                Email:
                    email,

                Username:
                    username,

                Password:
                    password,

                confirmPassword:
                    confirmPassword

            },


            success: function (response) {


                if (response.success) {


                    alert(
                        "INSERTED DATA\n\n" +
                        "Full Name: " +
                        fullName +
                        "\n" +
                        "Email: " +
                        email +
                        "\n" +
                        "Username: " +
                        username
                    );


                    $("#registerForm")[0]
                        .reset();


                }
                else {


                    alert(
                        response.message
                    );


                }

            },


            error: function (xhr) {


                console.log(
                    xhr.responseText
                );


                alert(
                    "Server Error: " +
                    xhr.status
                );


            }

        });

    });


});