use buttplug::{
    client::{ButtplugClient, ScalarValueCommand},
    core::connector::new_json_ws_client_connector,
};
use clap::Parser;
use miette::{IntoDiagnostic, Result};
use tokio::{fs::{self, File}, io::AsyncReadExt};
use std::{thread, time::Duration};

/// Native connector for StickFuckTheMod
#[derive(Parser, Debug)]
#[command(about)]
struct Args {
    /// Websocket address of the Buttplug server to connect to
    ws_addr: String,
}

#[tokio::main]
async fn main() -> Result<()> {
    tracing_subscriber::fmt::init();

    let args = Args::parse();

    #[cfg(unix)]
    let path = {
        let home = dirs_sys::home_dir().expect("$HOME is not set");
        home.join(".steam/root/steamapps/common/StickFightTheGame/buttplug.commands")
    };

    #[cfg(windows)]
    let path = {
        let appdata = dirs_sys::known_folder_roaming_app_data().unwrap();
        appdata.join(todo!("figure out what the path is on windows"))
    };

    let client = ButtplugClient::new("StickFuck");
    let connector = new_json_ws_client_connector(&args.ws_addr);
    client.connect(connector).await.into_diagnostic()?;

    client.start_scanning().await.into_diagnostic()?;

    let mut current_intensity: f64 = 0.0;

    loop {
        if let Ok(mut file) = File::open(&path).await {
            let mut buf = String::new();
            if file.read_to_string(&mut buf).await.is_ok() {
                println!("{}", buf);
                let mut args = buf.split(" ");
                match args.next() {
                    Some("v") =>  if let Some(Ok(intensity)) = args.next().map(str::parse::<f64>) {
                        current_intensity += intensity;
                        println!("{}", intensity);
                    },
                    Some("s") => {
                        for device in client.devices() {
                            device.vibrate(&ScalarValueCommand::ScalarValue(0.0)).await
                            .into_diagnostic()?;
                        }
                        current_intensity = 0.0;
                    },
                    _ => unimplemented!(),
                }
            }
            fs::remove_file(&path).await.into_diagnostic()?;
        }

        if current_intensity > 0.0 {
            for device in client.devices() {
                device
                    .vibrate(&ScalarValueCommand::ScalarValue(current_intensity.min(1.0)))
                    .await
                    .into_diagnostic()?;
            }
            current_intensity -= 0.05;
            if current_intensity <= 0.0 {
                for device in client.devices() {
                    device.vibrate(&ScalarValueCommand::ScalarValue(0.0)).await
                    .into_diagnostic()?;
                }
            }
        }

        thread::sleep(Duration::from_millis(50));
    }
}
