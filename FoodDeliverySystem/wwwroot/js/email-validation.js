document.addEventListener("DOMContentLoaded", () => {

    const emailInputs = document.querySelectorAll(".email-validation");

    emailInputs.forEach(email => {

        email.addEventListener("input", function () {

            const value = email.value.trim();

            const emailRegex =
                /^[A-Za-z0-9._%+-]+@[A-Za-z0-9-]+\.(com|net|org|edu|gov|mil|info|biz|co|io|me|dev|app|tech|ai|ng|co\.uk)$/i;

            if (value === "") {

                email.classList.remove("is-valid");
                email.classList.remove("is-invalid");

                return;
            }

            if (emailRegex.test(value)) {

                email.classList.remove("is-invalid");
                email.classList.add("is-valid");

            }
            else {

                email.classList.remove("is-valid");
                email.classList.add("is-invalid");

            }

        });

    });

});