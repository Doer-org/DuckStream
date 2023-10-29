resource "google_storage_bucket" "duckstream_strage" {
  name     = var.bucket_name
  location = var.default_region
  autoclass {
    enabled = true
  }
  uniform_bucket_level_access = true
}

# パブリックへのリードアクセスを設定
resource "google_storage_bucket_iam_member" "public_read_access" {
  bucket = google_storage_bucket.duckstream_strage.name
  role   = "roles/storage.objectViewer"
  member = "allUsers"
}
