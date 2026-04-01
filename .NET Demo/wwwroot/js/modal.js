(() => {
    const host = document.getElementById("modalHost");
    const key = host.dataset.modalKey;
    const modalElement = document.getElementById(`${key}Modal`);
    const modalContent = document.getElementById(`${key}ModalContent`);
    const modal = new bootstrap.Modal(modalElement);
    const urlBase = host.dataset.upsertUrl;

    const parseValidation = (scope) => {
        const forms = scope.querySelectorAll("form");
        forms.forEach((form) => {
            $(form).removeData("validator");
            $(form).removeData("unobtrusiveValidation");
        });

        $.validator.unobtrusive.parse(scope);
    };

    const loadModalContent = async (id) => {
        const query = id && id !== "0" ? `?id=${id}` : "";
        const response = await fetch(
            `${urlBase}${query}`, 
            {
                headers: 
                {
                    "X-Requested-With": "XMLHttpRequest"
                }
            });

        if (!response.ok) 
        {
            throw new Error("Unable to load the category form.");
        }

        modalContent.innerHTML = await response.text();
        parseValidation(modalContent);
        modal.show();
    };

    document.addEventListener("click", async (event) => 
    {
        const trigger = event.target.closest(`.js-${key}-modal-trigger`);
        if (!trigger) 
        {
            return;
        }

        try 
        {
            await loadModalContent(trigger.dataset.id);
        } 
        catch (error) 
        {
            console.error(error);
            toastr.error(`Unable to open the ${key} form.`);
        }
    });

    document.addEventListener("submit", async (event) => 
    {
        const form = event.target.closest(`#${key}UpsertForm`);
        if (!form) 
        {
            return;
        }

        event.preventDefault();
        parseValidation(form);

        if (!$(form).valid()) 
        {
            return;
        }

        try 
        {
            const response = await fetch(form.action, 
            {
                method: "POST",
                body: new FormData(form),
                headers: 
                {
                    "X-Requested-With": "XMLHttpRequest"
                }
            });

            const contentType = response.headers.get("content-type") || "";

            if (contentType.includes("application/json")) 
            {
                const result = await response.json();
                if (result.success) 
                {
                    modal.hide();
                    window.location.reload();
                }
                return;
            }

            modalContent.innerHTML = await response.text();
            parseValidation(modalContent);
            modal.show();
        } 
        catch (error) 
        {
            console.error(error);
            toastr.error(`Unable to save the ${key}.`);
        }
    });

    modalElement.addEventListener("hidden.bs.modal", () => 
    {
        modalContent.innerHTML = "";
    });
})();
