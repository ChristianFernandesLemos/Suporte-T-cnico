import React, { useState, useEffect } from 'react';
import { View, Text, SafeAreaView, ScrollView, ActivityIndicator, Alert } from 'react-native';
import Header from './Header';
import stylesdetailsScreen from '../styles/CallDetailsScreenStyles';
import { COLORS } from '../styles/GlobalStyles';
import { sendToN8n } from '../../components/requisicoes';

const CallDetailsScreen = ({ navigation, route }) => {
  const { callId } = route.params;
  const [callDetails, setCallDetails] = useState(null);
  const [isLoading, setIsLoading] = useState(true);

  // --- NEW HELPER FUNCTION ---
  // Converts the numeric status ID to the text label from your C# Enum
  const getStatusText = (statusId) => {
      // Ensure we compare numbers, even if the API sends a string "1"
      const id = Number(statusId);
      
      switch(id) {
          case 1: return "Aberto";
          case 2: return "Em Andamento";
          case 3: return "Resolvido";
          case 4: return "Fechado";
          case 5: return "Cancelado";
          default: return "Desconhecido"; // Fallback for unexpected numbers
      }
  };

  useEffect(() => {
    const fetchCallDetails = async () => {
      try {
        // Type 2 = Details
        const result = await sendToN8n(null, callId, 2, null, null);
        
        let data = null;
        // Logic to extract the data object (handles direct object or array)
        if (Array.isArray(result) && result.length > 0) {
            data = result[0];
        } else if (result && typeof result === 'object' && !Array.isArray(result)) {
             data = result;
        }

        // Check if a valid object with ID was found
        if (data && (data.id_chamado || data.Id_Chamado || data.id)) {
            setCallDetails({
                id: data.Id_Chamado || data.id_chamado || data.id,
                titulo: data.Titulo || data.titulo || "Sem Título",
                categoria: data.Categoria || data.categoria || "N/A",
                prioridade: data.Prioridade || data.prioridade || "Normal",
                descricao: data.Descricao || data.descricao || "Sem descrição.",
                
                // Date formatting (PT-BR)
                Data_Registro: data.Data_Registro ? new Date(data.Data_Registro).toLocaleDateString('pt-BR') : 'N/A',
                
                // --- UPDATE HERE ---
                // We use the helper function to display text instead of the number
                Status: getStatusText(data.Status || data.status),
                
                Solucao: data.Solucao || data.solucao || 'Ainda sem solução',
                Tecnico_Atribuido: data.Tecnico_Atribuido || 'Aguardando',
                Data_Resolucao: data.Data_Resolucao ? new Date(data.Data_Resolucao).toLocaleDateString('pt-BR') : 'Em andamento',
                cadastrador: data.Nome_Usuario || data.nome || data.Afetado || "Usuário", 
            });
        } else {
            Alert.alert("Aviso", "Detalhes não encontrados.");
            navigation.goBack();
        }
      } catch (err) {
        Alert.alert("Erro", "Falha ao conectar.");
        navigation.goBack();
      } finally {
        setIsLoading(false);
      }
    };

    if (callId) fetchCallDetails();
  }, [callId]);

  if (isLoading) {
    return (
      <View style={{ flex: 1, justifyContent: 'center', alignItems: 'center' }}>
        <ActivityIndicator size="large" color={COLORS.primary || 'blue'} />
      </View>
    );
  }

  if (!callDetails) return null;
  
  const DetailRow = ({ label, value }) => (
    <View style={stylesdetailsScreen.detailRow}>
      <Text style={stylesdetailsScreen.label}>{label}:</Text>
      <Text style={stylesdetailsScreen.value}>{value}</Text>
    </View>
  );

  return (
    <SafeAreaView style={stylesdetailsScreen.container}>
      <Header showBackButton={true} onBackPress={() => navigation.goBack()} />
      <ScrollView style={stylesdetailsScreen.content}>
        <View style={stylesdetailsScreen.detailsCard}>
          <Text style={stylesdetailsScreen.title}>Chamado #{callDetails.id}</Text>
          <View style={stylesdetailsScreen.detailsContainer}>
            <DetailRow label="Título" value={callDetails.titulo} />
            <DetailRow label="Categoria" value={callDetails.categoria} />
            <DetailRow label="Prioridade" value={callDetails.prioridade} />
            
            {/* Now this will show "Aberto", "Resolvido", etc. instead of 1, 3 */}
            <DetailRow label="Status" value={callDetails.Status} />
            
            <DetailRow label="Data Abertura" value={callDetails.Data_Registro} />
            
            <View style={{ height: 1, backgroundColor: '#EEE', marginVertical: 10 }} />
            
            <DetailRow label="Solicitante" value={callDetails.cadastrador} />
            <DetailRow label="Técnico" value={callDetails.Tecnico_Atribuido} />
            
            <View style={{ height: 1, backgroundColor: '#EEE', marginVertical: 10 }} />
            
            <DetailRow label="Descrição" value={callDetails.descricao} />
            
            {callDetails.Solucao && callDetails.Solucao !== 'Ainda sem solução' && (
                <>
                    <View style={{ height: 1, backgroundColor: '#EEE', marginVertical: 10 }} />
                    <Text style={[stylesdetailsScreen.label, { color: 'green', marginTop: 5 }]}>Solução Técnica:</Text>
                    <Text style={stylesdetailsScreen.value}>{callDetails.Solucao}</Text>
                    <DetailRow label="Data Resolução" value={callDetails.Data_Resolucao} />
                </>
            )}
           </View>
        </View>
      </ScrollView>
    </SafeAreaView>
  );
};
export default CallDetailsScreen;