using System;
using UnityEngine;
using Reactor.Networking;
using Hazel;

namespace SukunaMod
{
    public static class SukunaRPC
    {
        // RPC Call IDs (use high IDs to avoid conflicts)
        private const byte RPC_Transform = 150;
        private const byte RPC_Cleave = 151;
        private const byte RPC_Slash = 152;
        private const byte RPC_Domain = 153;

        public static void SendTransformRPC(byte playerId, bool isSukuna)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                RPC_Transform,
                SendOption.Reliable,
                -1
            );

            writer.Write(playerId);
            writer.Write(isSukuna);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void SendCleaveRPC(byte playerId, Vector3 position, float radius)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                RPC_Cleave,
                SendOption.Reliable,
                -1
            );

            writer.Write(playerId);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(radius);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void SendSlashRPC(byte playerId, byte targetId)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                RPC_Slash,
                SendOption.Reliable,
                -1
            );

            writer.Write(playerId);
            writer.Write(targetId);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void SendDomainRPC(byte playerId, Vector3 position, float radius, float duration)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(
                PlayerControl.LocalPlayer.NetId,
                RPC_Domain,
                SendOption.Reliable,
                -1
            );

            writer.Write(playerId);
            writer.Write(position.x);
            writer.Write(position.y);
            writer.Write(radius);
            writer.Write(duration);

            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void HandleRPC(byte callId, MessageReader reader)
        {
            switch (callId)
            {
                case RPC_Transform:
                    HandleTransformRPC(reader);
                    break;
                case RPC_Cleave:
                    HandleCleaveRPC(reader);
                    break;
                case RPC_Slash:
                    HandleSlashRPC(reader);
                    break;
                case RPC_Domain:
                    HandleDomainRPC(reader);
                    break;
            }
        }

        private static void HandleTransformRPC(MessageReader reader)
        {
            byte playerId = reader.ReadByte();
            bool isSukuna = reader.ReadBoolean();

            var sukuna = SukunaMod.Instance.GetSukunaPlayer(playerId);
            if (sukuna != null)
            {
                sukuna.ToggleTransformation();
            }

            SukunaMod.Logger.LogInfo($"Player {playerId} is now {(isSukuna ? "Sukuna" : "normal")}");
        }

        private static void HandleCleaveRPC(MessageReader reader)
        {
            byte playerId = reader.ReadByte();
            float posX = reader.ReadSingle();
            float posY = reader.ReadSingle();
            float radius = reader.ReadSingle();

            Vector3 position = new Vector3(posX, posY, 0);

            // Visual effect (spawn particle effect at position)
            SukunaMod.Logger.LogInfo($"Cleave executed at {position} with radius {radius}");
        }

        private static void HandleSlashRPC(MessageReader reader)
        {
            byte playerId = reader.ReadByte();
            byte targetId = reader.ReadByte();

            SukunaMod.Logger.LogInfo($"Slash: Player {playerId} hit Player {targetId}");
        }

        private static void HandleDomainRPC(MessageReader reader)
        {
            byte playerId = reader.ReadByte();
            float posX = reader.ReadSingle();
            float posY = reader.ReadSingle();
            float radius = reader.ReadSingle();
            float duration = reader.ReadSingle();

            Vector3 position = new Vector3(posX, posY, 0);

            var sukuna = SukunaMod.Instance.GetSukunaPlayer(playerId);
            if (sukuna != null)
            {
                sukuna.ActivateDomain(duration, radius);
            }

            SukunaMod.Logger.LogInfo($"Domain Expansion at {position} radius {radius} for {duration}s");
        }
    }
}
