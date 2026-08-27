$(document).ready(function () {

    // =========================
    // LOGIN
    // =========================
    $("#loginForm").submit(function (e) {

        e.preventDefault();

        const username =
            $("#loginUsername").val().trim();

        const password =
            $("#loginPassword").val().trim();


        if (!username || !password) {

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



    // =========================
    // REGISTER
    // =========================
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


        if (password !== confirmPassword) {

            alert(
                "Passwords do not match."
            );

            return;
        }


        $.ajax({

            url: "/Home/register",

            type: "POST",

            data: {

                FullName: fullName,

                Email: email,

                Username: username,

                Password: password,

                confirmPassword:
                    confirmPassword

            },


            success: function (response) {

                if (response.success) {

                    alert(

                        "INSERTED DATA\n\n" +

                        "Full Name: " +
                        response.fullName +
                        "\n" +

                        "Email: " +
                        response.email +
                        "\n" +

                        "Username: " +
                        response.username

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



    // =========================
    // UPDATE USER
    // =========================
    $("#editForm").submit(function (e) {

        e.preventDefault();


        const id =
            $("#editId").val();


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



    // =========================
    // DELETE USER
    // =========================
    $(document).on(
        "click",
        ".deleteUser",
        function () {

            const id =
                $(this).data("id");


            if (!id) {

                alert(
                    "Invalid user ID."
                );

                return;
            }


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


                        $("#userRow-" + id)
                            .remove();

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

});