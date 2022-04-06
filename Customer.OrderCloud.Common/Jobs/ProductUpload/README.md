# Product Upload

The ProductUpload feature was created to demonstrate how to take a data source and use it to build a set of products on OrderCloud.  In this demo we use a example_products.json file.  The proces works by reading the sample data and transsforming it into OrderCloud C# classes.  Then products are grouped together and uploaded to OrderCloud.

When uploading products to OrderCloud it is important to follow the basic order of:
1. Build The Price Schedule
2. Build The Product
3. Build a Spec (if needed)
4. Build Spec Options (if needed)
5. Assign Specs to Product
6. Generate Variants (if needed)
7. Modifying Variants (if needed)

Addtional steps may include creating Variants and Assigning Products to Buyer Groups.
