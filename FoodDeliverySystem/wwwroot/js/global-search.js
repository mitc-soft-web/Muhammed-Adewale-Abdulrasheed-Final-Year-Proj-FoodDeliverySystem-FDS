document.addEventListener("DOMContentLoaded", () => {

    const input = document.getElementById("globalSearch");
    const suggestions = document.getElementById("globalSuggestions");

    if (!input || !suggestions) {
        return;
    }

    const form = input.closest("form");

    if (!form) {
        return;
    }


    // =========================================================
    // HELPERS
    // =========================================================

    function getAntiForgeryToken() {

        return document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value || "";
    }


    function escapeHtml(value) {

        if (value === null || value === undefined) {
            return "";
        }

        return String(value)
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }


    function encodeSearch(text) {

        return encodeURIComponent(
            (text || "").trim()
        );
    }


    function showToast(message) {

        const toast =
            document.getElementById("toastMessage");

        if (!toast) {
            return;
        }

        toast.innerText = message;

        toast.classList.add("show");

        setTimeout(() => {

            toast.classList.remove("show");

        }, 2500);
    }


    function hideSuggestions() {

        suggestions.style.display = "none";
        suggestions.innerHTML = "";
    }


    function showSuggestions() {

        suggestions.style.display = "block";
    }


    // =========================================================
    // SAVE SEARCH
    // =========================================================

    async function saveSearch(keyword) {

        keyword = (keyword || "").trim();

        if (!keyword) {
            return false;
        }

        try {

            const token = getAntiForgeryToken();

            const formData = new URLSearchParams();

            formData.append("text", keyword);

            const response = await fetch("/Search/Save", {

                method: "POST",

                headers: {
                    "Content-Type":
                        "application/x-www-form-urlencoded; charset=UTF-8",

                    "RequestVerificationToken":
                        token
                },

                body: formData.toString()
            });

            return response.ok;

        }
        catch (error) {

            console.error(
                "Unable to save search history:",
                error
            );

            return false;
        }
    }


    // =========================================================
    // FIND THE BEST DESTINATION FOR A STORED SEARCH
    // =========================================================
    //
    // Recent/Trending currently store only the search text.
    //
    // We therefore ask the existing Suggestions endpoint what
    // that text represents.
    //
    // If it is an exact restaurant name, go directly to that
    // restaurant instead of sending the user to food search.
    //
    // Otherwise fall back to the existing Search results page.
    //
    // This keeps the existing DTO/backend unchanged.
    // =========================================================

    async function resolveSearchDestination(text) {

        text = (text || "").trim();

        if (!text) {
            return "/Search";
        }

        try {

            const response = await fetch(
                `/Search/Suggestions?keyword=${encodeSearch(text)}`
            );

            if (!response.ok) {
                return `/Search?keyword=${encodeSearch(text)}`;
            }

            const results = await response.json();

            if (Array.isArray(results)) {

                const normalizedText =
                    text.toLowerCase();

                // -------------------------------------------------
                // 1. Exact restaurant match gets priority.
                // -------------------------------------------------

                const restaurant =
                    results.find(item =>
                        item.type === "Restaurant" &&
                        (item.text || "")
                            .trim()
                            .toLowerCase() === normalizedText
                    );

                if (restaurant &&
                    restaurant.restaurantId) {

                    return `/Restaurant/Details/${restaurant.restaurantId}`;
                }


                // -------------------------------------------------
                // 2. Exact food match.
                // -------------------------------------------------

                const food =
                    results.find(item =>
                        item.type === "Food" &&
                        (item.text || "")
                            .trim()
                            .toLowerCase() === normalizedText
                    );

                if (food &&
                    food.restaurantId) {

                    return `/Restaurant/Details/${food.restaurantId}`;
                }


                // -------------------------------------------------
                // 3. Exact category match.
                // -------------------------------------------------

                const category =
                    results.find(item =>
                        item.type === "Category" &&
                        (item.text || "")
                            .trim()
                            .toLowerCase() === normalizedText
                    );

                if (category) {

                    return `/Search?keyword=${encodeSearch(text)}`;
                }
            }

        }
        catch (error) {

            console.error(
                "Unable to resolve search destination:",
                error
            );
        }

        // ---------------------------------------------------------
        // 4. Safe fallback.
        // ---------------------------------------------------------

        return `/Search?keyword=${encodeSearch(text)}`;
    }


    // =========================================================
    // OPEN A STORED SEARCH
    // =========================================================

    async function openStoredSearch(text) {

        text = (text || "").trim();

        if (!text) {
            return;
        }

        input.value = text;

        const destination =
            await resolveSearchDestination(text);

        window.location.href = destination;
    }


    // =========================================================
    // SEARCH SUGGESTIONS
    // =========================================================

    input.addEventListener("input", async function () {

        const keyword =
            this.value.trim();

        // Empty search box
        if (!keyword) {

            await loadRecentTrending();

            return;
        }


        // Don't search for a single character
        if (keyword.length < 2) {

            hideSuggestions();

            return;
        }


        try {

            const response = await fetch(
                `/Search/Suggestions?keyword=${encodeSearch(keyword)}`
            );

            if (!response.ok) {

                hideSuggestions();

                return;
            }

            const data = await response.json();

            suggestions.innerHTML = "";


            // -----------------------------------------------------
            // No results
            // -----------------------------------------------------

            if (!Array.isArray(data) ||
                data.length === 0) {

                suggestions.innerHTML = `
                    <div class="search-item search-no-results">
                        <div class="search-info">
                            <div class="search-title">
                                No results found
                            </div>
                        </div>
                    </div>
                `;

                showSuggestions();

                return;
            }


            // -----------------------------------------------------
            // Render suggestions
            // -----------------------------------------------------

            data.forEach(item => {

                let url = "#";

                if (item.type === "Food") {

                    url =
                        `/Restaurant/Details/${item.restaurantId}`;
                }

                else if (item.type === "Restaurant") {

                    url =
                        `/Restaurant/Details/${item.restaurantId}`;
                }

                else if (item.type === "Category") {

                    url =
                        `/Search?keyword=${encodeSearch(item.text)}`;
                }


                suggestions.innerHTML += `

                    <a href="${url}"
                       class="search-item live-search-item"
                       data-keyword="${escapeHtml(item.text)}">

                        <div class="search-info">

                            <div class="search-title">
                                ${escapeHtml(item.text)}
                            </div>

                            <small>
                                ${escapeHtml(item.subtitle)}
                            </small>

                        </div>

                    </a>
                `;
            });


            showSuggestions();


            // -----------------------------------------------------
            // Live suggestion click
            // -----------------------------------------------------

            suggestions
                .querySelectorAll(".live-search-item")
                .forEach(item => {

                    item.addEventListener(
                        "click",
                        async function (event) {

                            event.preventDefault();

                            const keyword =
                                this.dataset.keyword;

                            if (keyword) {

                                await saveSearch(keyword);

                                window.location.href =
                                    this.href;
                            }
                        }
                    );
                });

        }
        catch (error) {

            console.error(
                "Search suggestions failed:",
                error
            );

            hideSuggestions();
        }
    });


    // =========================================================
    // SEARCH BOX FOCUS
    // =========================================================

    input.addEventListener("focus", async function () {

        if (this.value.trim() === "") {

            await loadRecentTrending();
        }
    });


    // =========================================================
    // FORM SUBMIT
    // =========================================================
    //
    // IMPORTANT:
    // We prevent the browser's immediate navigation so that the
    // search is saved BEFORE Search/Index loads.
    // =========================================================

    form.addEventListener("submit", async function (event) {

        event.preventDefault();

        const keyword =
            input.value.trim();

        if (!keyword) {

            return;
        }

        const saved =
            await saveSearch(keyword);

        if (!saved) {

            console.warn(
                "Search could not be saved. Continuing normally."
            );
        }

        sessionStorage.setItem(
            "foodXpressSearchToast",
            `Showing search results for "${keyword}".`
        );

        window.location.href =
            `/Search?keyword=${encodeSearch(keyword)}`;
    });


    // =========================================================
    // LOAD RECENT + TRENDING
    // =========================================================

    async function loadRecentTrending() {

        suggestions.innerHTML = "";

        let recent = [];
        let trending = [];


        // ---------------------------------------------------------
        // RECENT SEARCHES
        // ---------------------------------------------------------

        try {

            const recentResponse =
                await fetch("/Search/Recent");

            if (recentResponse.ok) {

                recent =
                    await recentResponse.json();
            }

        }
        catch (error) {

            console.error(
                "Unable to load recent searches:",
                error
            );
        }


        if (Array.isArray(recent) &&
            recent.length > 0) {

            suggestions.innerHTML += `

                <div class="search-header">

                    <span>
                        Recent Searches
                    </span>

                    <button type="button"
                            id="clearHistoryBtn"
                            class="clear-history">

                        Clear All

                    </button>

                </div>
            `;


            recent.forEach(item => {

                const text =
                    item.text || "";

                suggestions.innerHTML += `

                    <a href="#"
                       class="search-item recent-search"
                       data-text="${escapeHtml(text)}">

                        <div class="search-info">

                            <div class="search-title">

                                <i class="bi bi-clock-history"></i>

                                ${escapeHtml(text)}

                            </div>

                        </div>

                    </a>
                `;
            });
        }


        // ---------------------------------------------------------
        // TRENDING SEARCHES
        // ---------------------------------------------------------

        try {

            const trendingResponse =
                await fetch("/Search/Trending");

            if (trendingResponse.ok) {

                trending =
                    await trendingResponse.json();
            }

        }
        catch (error) {

            console.error(
                "Unable to load trending searches:",
                error
            );
        }


        if (Array.isArray(trending) &&
            trending.length > 0) {

            suggestions.innerHTML += `

                <div class="search-header">

                    <span>
                        Trending Searches
                    </span>

                </div>
            `;


            trending.forEach(item => {

                const text =
                    item.text || "";

                suggestions.innerHTML += `

                    <a href="#"
                       class="search-item trending-search"
                       data-text="${escapeHtml(text)}">

                        <div class="search-info">

                            <div class="search-title">

                                <i class="bi bi-fire"></i>

                                ${escapeHtml(text)}

                            </div>

                        </div>

                    </a>
                `;
            });
        }


        // ---------------------------------------------------------
        // Nothing available
        // ---------------------------------------------------------

        if ((!Array.isArray(recent) ||
            recent.length === 0) &&
            (!Array.isArray(trending) ||
                trending.length === 0)) {

            suggestions.innerHTML = `

                <div class="search-item search-empty">

                    <div class="search-info">

                        <div class="search-title">
                            Start searching for food,
                            restaurants or categories
                        </div>

                    </div>

                </div>
            `;
        }


        showSuggestions();


        // =========================================================
        // RECENT SEARCH CLICK
        // =========================================================

        suggestions
            .querySelectorAll(".recent-search")
            .forEach(item => {

                item.addEventListener(
                    "click",
                    async function (event) {

                        event.preventDefault();

                        await openStoredSearch(
                            this.dataset.text
                        );
                    }
                );
            });


        // =========================================================
        // TRENDING SEARCH CLICK
        // =========================================================

        suggestions
            .querySelectorAll(".trending-search")
            .forEach(item => {

                item.addEventListener(
                    "click",
                    async function (event) {

                        event.preventDefault();

                        const text =
                            this.dataset.text;

                        await saveSearch(text);

                        const destination =
                            await resolveSearchDestination(text);

                        window.location.href =
                            destination;
                    }
                );
            });


        // =========================================================
        // CLEAR ALL
        // =========================================================

        const clearBtn =
            document.getElementById("clearHistoryBtn");


        if (clearBtn) {

            clearBtn.addEventListener(
                "click",
                async function (event) {

                    event.preventDefault();
                    event.stopPropagation();

                    const token =
                        getAntiForgeryToken();


                    try {

                        const response =
                            await fetch(
                                "/Search/ClearHistory",
                                {
                                    method: "POST",

                                    headers: {
                                        "RequestVerificationToken":
                                            token
                                    }
                                }
                            );


                        if (response.ok) {

                            showToast(
                                "Recent searches cleared successfully."
                            );

                            await loadRecentTrending();

                            return;
                        }


                        showToast(
                            "Unable to clear recent searches."
                        );

                    }
                    catch (error) {

                        console.error(
                            "Unable to clear search history:",
                            error
                        );

                        showToast(
                            "Unable to clear recent searches."
                        );
                    }
                }
            );
        }
    }


    // =========================================================
    // CLOSE DROPDOWN WHEN CLICKING OUTSIDE
    // =========================================================

    document.addEventListener("click", function (event) {

        if (!input.contains(event.target) &&
            !suggestions.contains(event.target)) {

            hideSuggestions();
        }
    });


    // =========================================================
    // SHOW SEARCH TOAST AFTER PAGE NAVIGATION
    // =========================================================
    //
    // sessionStorage lets the toast survive the navigation from
    // the search box to Search/Index.
    // =========================================================

    const pendingSearchToast =
        sessionStorage.getItem("foodXpressSearchToast");


    if (pendingSearchToast) {

        sessionStorage.removeItem(
            "foodXpressSearchToast"
        );

        showToast(pendingSearchToast);
    }

});