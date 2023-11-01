CREATE TABLE gcs_image
(
    img_id     varchar(255) not null,
    img_url    varchar(255) not null,
    created_at timestamptz  not null,
    PRIMARY KEY (img_id)
);

CREATE TABLE inference_result
(
    input_img_id     varchar(255) not null,
    result_img_id    varchar(255) not null,
    prompt           varchar(255) not null,
    converted_prompt varchar(255) not null,
    PRIMARY KEY (input_img_id, result_img_id),
    FOREIGN KEY (input_img_id) REFERENCES gcs_image (img_id),
    FOREIGN KEY (result_img_id) REFERENCES gcs_image (img_id)
);
