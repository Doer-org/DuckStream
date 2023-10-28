provider "google" {
  credentials = var.credential_path
  project     = var.project_id
  region      = var.default_region
}
