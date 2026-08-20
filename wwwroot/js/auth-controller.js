$(document).ready(function () {

    // =========================
    // LOGIN
    // =========================
    $("#loginForm").submit(function (e) {

        e.preventDefault();

        const username = $("#loginUsername").val().trim();
        const password = $("#loginPassword").val().trim();

        if (!username || !password) {
            alert("Please enter username and password.");
            return;
        }

        $.ajax({
            url: "/Home/Login",
            type: "POST",

            data: {
                Username: username,
                Password: password
            },

            success: function (response) {

                if (response.success) {

                    alert(response.message);

                    window.location.href = "/Home/Index";

                } else {

                    alert(response.message);
                }
            },

            error: function (xhr) {

                console.log(xhr.responseText);

                alert("Server Error: " + xhr.status);
            }
        });

    });


    // =========================
    // REGISTER
    // =========================
    $("#registerForm").submit(function (e) {

        e.preventDefault();

        const fullName = $("#registerFullName").val().trim();
        const email = $("#registerEmail").val().trim();
        const username = $("#registerUsername").val().trim();
        const password = $("#registerPassword").val();
        const confirmPassword = $("#registerConfirmPassword").val();

        if (!fullName ||
            !email ||
            !username ||
            !password ||
            !confirmPassword) {

            alert("Please complete all fields.");
            return;
        }

        if (password !== confirmPassword) {

            alert("Passwords do not match.");
            return;
        }

        $.ajax({
            url: "/Home/Register",
            type: "POST",

            data: {
                FullName: fullName,
                Email: email,
                Username: username,
                Password: password,
                confirmPassword: confirmPassword
            },

            success: function (response) {

                if (response.success) {

                    alert(
                        "INSERTED DATA\n\n" +
                        "Full Name: " + response.fullName + "\n" +
                        "Email: " + response.email + "\n" +
                        "Username: " + response.username
                    );

                    $("#registerForm")[0].reset();

                } else {

                    alert(response.message);
                }
            },

            error: function (xhr) {

                console.log(xhr.responseText);

                alert("Server Error: " + xhr.status);
            }
        });

    });

});