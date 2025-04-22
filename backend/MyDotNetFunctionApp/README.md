# 🔧 Azure Function: Counter with Cosmos DB

This C# Azure Function defines a **serverless HTTP-triggered function** named `Counter` that:

- Connects to **Azure Cosmos DB**
- Tracks and stores how many times the function has been invoked
- Returns the **current invocation count** in a JSON response

---

## 🔍 What Happens in Detail

### 1. Initialization and Setup

The constructor `TestHttp()` performs two tasks:

- Retrieves the Cosmos DB connection string from the environment variable `CosmosDbConnectionString`
- Initializes a `CosmosClient` and connects to the container `myContainer` in the `myDatabase` database

---

### 2. Resource Validation

The method `EnsureCosmosResourcesExistAsync` ensures that:

- The **Cosmos DB database** exists (or is created)
- The **Cosmos DB container** exists with `/id` as the partition key (or is created)

---

### 3. HTTP Trigger Function: `Run()`

- Triggered via **HTTP GET or POST**
- Logs request activity
- Validates that the Cosmos resources are available

---

### 4. Counter Logic

- Attempts to **read** an item from the Cosmos container with `id = "functionCounter"`
- If found, it **increments** the `count` property
- If not found, it **creates** a new item with `count = 1`

```json
{
  "id": "functionCounter",
  "count": 3
}
```

---

### 5. Upsert Operation

Uses `UpsertItemAsync()` to either insert or update the counter item in Cosmos DB.

---

### 6. Response

Returns a JSON object with the updated count and ID:

```json
{
  "count": 3,
  "id": "functionCounter"
}
```

---

### 7. Error Handling

- Catches and logs Cosmos DB exceptions with appropriate status codes (e.g., `NotFound`, `Unauthorized`)
- Catches general exceptions and returns an HTTP 500 response

---

## ✅ Summary Table

| Component                          | Purpose                                                    |
|-----------------------------------|------------------------------------------------------------|
| `TestHttp` class                  | Azure Function class handling HTTP requests                |
| `CosmosClient`                   | Connects to Azure Cosmos DB                                |
| `InvocationCounter` class        | Data model for the counter document                        |
| `Run()` method                   | Main HTTP-triggered function                               |
| Cosmos DB container              | Stores the single counter document                         |
| `UpsertItemAsync()`             | Inserts or updates the counter document in Cosmos DB       |
| `EnsureCosmosResourcesExistAsync()` | Ensures DB and container exist before any operations     |
