# resource "google_storage_bucket" "duckstream_storage" {
#   name     = var.bucket_name
#   location = var.default_region
#   autoclass {
#     enabled = true
#   }
#   uniform_bucket_level_access = true
# }
# resource "google_storage_bucket_iam_member" "public_read_access" {
#   bucket = google_storage_bucket.duckstream_storage.name
#   role   = "roles/storage.objectViewer"
#   member = "allUsers"
# }

# 既存のバケットを参照する
data "google_storage_bucket" "duckstream_storage" {
  name = var.bucket_name
}

resource "google_storage_bucket_iam_member" "public_read_access" {
  bucket = data.google_storage_bucket.duckstream_storage.name
  role   = "roles/storage.objectViewer"
  member = "allUsers"
}
