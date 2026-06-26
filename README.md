#Order Processing System
<img width="1702" height="666" alt="image" src="https://github.com/user-attachments/assets/8f2e7e73-9ca2-4184-80ff-73bebc3b2fcf" />
<img width="917" height="804" alt="image" src="https://github.com/user-attachments/assets/2ecd29d6-b82d-4232-a443-7d850a7bc943" />
<img width="1549" height="634" alt="image" src="https://github.com/user-attachments/assets/8a7c2502-3f40-4f7d-bbcb-79fbf4e00d0b" />
<img width="1235" height="642" alt="image" src="https://github.com/user-attachments/assets/64917d19-8089-446b-980d-a7db6806ef57" />

<h4>
  When a client sends a request through Swagger, the API creates a new order and stores it in SQL Server with the status Pending. Immediately after saving, it publishes an orders/created event to an in-memory message bus. A background worker subscribed to this event picks it up, waits for 9 seconds to simulate a long-running business process such as payment verification or inventory checking, and then updates the order status from Pending to Processed. This event-driven approach keeps the API responsive while offloading time-consuming tasks to a background worker.
</h4>
