terraform {
  backend "gcs" {
    bucket = "duckstream_tfstate_bucket"
    prefix = "terraform/state"
  }
}
