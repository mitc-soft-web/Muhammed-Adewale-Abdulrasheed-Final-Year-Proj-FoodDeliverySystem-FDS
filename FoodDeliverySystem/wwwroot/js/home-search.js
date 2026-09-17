document.addEventListener("DOMContentLoaded", () => {

    const input = document.getElementById("searchInput");

    if (!input) return;

    const cards = document.querySelectorAll("#foodContainer > div");
    const suggestionBox = document.getElementById("searchSuggestions");
    const noResults = document.getElementById("noResults");
    const counter = document.getElementById("searchCount");

    function extractData(card) {

        return {

            food: card.dataset.food || "",

            restaurant: card.dataset.restaurant || "",

            category: card.dataset.category || "",

            description: card.dataset.description || "",

            price: card.dataset.price || "",

            available: card.dataset.status || ""

        };

    }

    function buildSearchText(data) {

        return (

            data.food + " " +

            data.restaurant + " " +

            data.category + " " +

            data.description + " " +

            data.price + " " +

            data.available + " " +

            data.delivery

        ).toLowerCase();

    }

    function filterFoods(keyword) {

        keyword = keyword.trim().toLowerCase();

        let visible = 0;

        cards.forEach(card => {

            const data = extractData(card);

            let searchText = buildSearchText(data);

            const aliases = {

                rice:
                    "jollof fried coconut ofada rice",

                swallow:
                    "amala eba semo fufu pounded yam",

                soup:
                    "egusi ogbono ewedu okra afang bitterleaf",

                meat:
                    "beef chicken turkey goat suya",

                drink:
                    "coke pepsi fanta sprite water juice",

                breakfast:
                    "bread tea coffee pancake",

                snack:
                    "burger shawarma pizza meatpie sausage roll",

                seafood:
                    "fish prawns shrimp crab",

                local:
                    "amala eba semo egusi ofada ewedu",

                fastfood:
                    "burger pizza fries chicken shawarma"

            };

            for (const key in aliases) {

                if (searchText.includes(aliases[key])) {

                    searchText += " " + key;

                }

            }

            if (keyword === "" || searchText.includes(keyword)) {

                card.style.display = "";

                card.style.opacity = "1";

                visible++;

            }

            else {

                card.style.display = "none";

            }

        });

        if (counter) {

            if (keyword === "") {

                counter.innerHTML = `Showing ${visible} meals`;

            }

            else {

                counter.innerHTML = `Showing ${visible} result(s)`;

            }

        }

        if (noResults) {

            noResults.style.display = visible === 0 ? "block" : "none";

        }

    }

    function buildSuggestions(keyword) {

        keyword = keyword.toLowerCase();

        suggestionBox.innerHTML = "";

        if (keyword === "") {

            suggestionBox.style.display = "none";

            return;

        }

        let items = [];

        cards.forEach(card => {

            const data = extractData(card);

            Object.values(data).forEach(value => {

                if (value.toLowerCase().includes(keyword)) {

                    items.push(value);

                }

            });

        });

        items = [...new Set(items)].slice(0, 8);

        if (items.length === 0) {

            suggestionBox.style.display = "none";

            return;

        }

        items.forEach(item => {

            const div = document.createElement("div");

            div.className = "search-item";

            div.innerHTML = item;

            div.onclick = function () {

                input.value = item;

                filterFoods(item);

                suggestionBox.style.display = "none";

            };

            suggestionBox.appendChild(div);

        });

        suggestionBox.style.display = "block";

    }

    input.addEventListener("input", function () {

        filterFoods(this.value);

        buildSuggestions(this.value);

    });

    document.addEventListener("click", function (e) {

        if (!input.contains(e.target) &&

            !suggestionBox.contains(e.target)) {

            suggestionBox.style.display = "none";

        }

    });

});