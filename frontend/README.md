### 📊 Azure Static Website Counter with Azure Functions & Cosmos DB

This project implements a **dynamic page view counter** on a static website hosted in **Azure Blob Storage** using the following components:

- HTML front end with a `#counter` placeholder
- JavaScript logic (`main.js`) to fetch and display the count
- Azure **HTTP-triggered Function** as an API
- Azure **Cosmos DB** for storing invocation count

---

## 💡 How It Works

### 1. HTML Integration

In your `index.html`, the view count is displayed inside the **hero section** using an anchor tag:

```html
<h3>
  Thanks for stopping by! This page has been viewed 
  <a id="counter"></a> times 😊
</h3>
```

This sets up an **empty placeholder** with `id="counter"` that will be updated dynamically after the page loads.

---

### 2. JavaScript (`main.js`)

Your `main.js` script contains the logic to call the Azure Function and update the DOM.

```javascript
window.addEventListener("DOMContentLoaded", (event) => {
  getVisitCount();
});

const functionApiApiUrl = "https://fn6ic.azurewebsites.net/api/Counter?code=YOUR_FUNCTION_KEY";

const getVisitCount = () => {
  let count = 30;
  fetch(functionApiApiUrl)
    .then((response) => response.json())
    .then((response) => {
      console.log("Website called function API.");
      count = response.count;
      document.getElementById("counter").innerText = count;
    })
    .catch(function (error) {
      console.log("Error fetching counter:", error);
    });

  return count;
};
```

---

### 3. Azure Function Logic (Serverless Backend)

The function is deployed in Azure and:

- Is triggered via **HTTP GET** requests
- Reads a document from **Cosmos DB** with `id = "functionCounter"`
- Increments the `count` value
- Returns the updated count as a JSON response:
  
```json
{
  "id": "functionCounter",
  "count": 123
}
```

---

## 🔄 End-to-End Flow

| Step | Description |
|------|-------------|
| 1.  | User visits the static site (served via Azure Blob Storage) |
| 2.  | `main.js` calls the Azure Function via `fetch()` |
| 3.  | Azure Function updates the counter in Cosmos DB |
| 4.  | Function returns the new count as a JSON response |
| 5.  | `main.js` updates the DOM element `#counter` with the count |

---

This pattern showcases a modern **serverless + static website** approach for lightweight telemetry without needing a full backend. 🚀
