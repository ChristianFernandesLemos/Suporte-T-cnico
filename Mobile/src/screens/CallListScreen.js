import React, { useState, useEffect } from 'react';
import {
  View, Text, SafeAreaView, TouchableOpacity, FlatList, ActivityIndicator, Alert
} from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import Header from './Header';
import CallListStyle from '../styles/CallListScreenStyle';
import { sendToN8n } from '../../components/requisicoes';

const ITEMS_PER_PAGE = 7; // Limite de 7 chamados por página
const MAX_VISIBLE_PAGES = 4; // Máximo de botões numéricos visíveis

const CallListScreen = ({ navigation, route }) => {
  const [currentPage, setCurrentPage] = useState(1);
  const [calls, setCalls] = useState([]); 
  const [loading, setLoading] = useState(true);

  // Extração segura do ID
  const { idusuario } = route.params || {};

  useEffect(() => {
    const fetchCalls = async () => {
      if (!idusuario) {
          Alert.alert("Erro", "ID do usuário não identificado.");
          setLoading(false);
          return;
      }

      setLoading(true);
      try {
        // Tipo 1 = Listar Chamados
        const response = await sendToN8n(idusuario, null, 1, null, null);

        let listaDeChamados = [];
        if (Array.isArray(response)) {
            listaDeChamados = response;
        } else if (response && response.data && Array.isArray(response.data)) {
            listaDeChamados = response.data;
        } else if (response && response.chamados) {
             listaDeChamados = response.chamados;
        }

        setCalls(listaDeChamados);
        setCurrentPage(1); // Reseta para a página 1 ao carregar novos dados
      } catch (error) {
        console.error(error);
        Alert.alert("Erro", "Falha ao conectar com o servidor.");
      } finally {
        setLoading(false);
      }
    };
    fetchCalls();
  }, [idusuario]); 

  // --- LÓGICA DE PAGINAÇÃO ---
  
  // 1. Calcular total de páginas
  const totalPages = Math.ceil(calls.length / ITEMS_PER_PAGE) || 1;

  // 2. Filtrar os dados para exibir apenas os da página atual
  const startIndex = (currentPage - 1) * ITEMS_PER_PAGE;
  const endIndex = startIndex + ITEMS_PER_PAGE;
  const currentData = calls.slice(startIndex, endIndex);

  const renderCallRow = ({ item }) => {
    const idDisplay = item.Id_Chamado || item.id_chamado || item.id || "#";
    const titleDisplay = item.Titulo || item.titulo || item.title || "Sem Título";
    const statusDisplay = item.Status || item.status || "Pendente";

    return (
        <View style={CallListStyle.tableRow}>
            <View style={CallListStyle.actionsCell}>
                <TouchableOpacity
                style={CallListStyle.iconButton}
                onPress={() => navigation.navigate('CallDetailsScreen', { callId: idDisplay })}
                >
                <Ionicons name="eye-outline" size={20} color="#000" />
                </TouchableOpacity>
            </View>
            <Text style={CallListStyle.cell}>{idDisplay}</Text>
            <Text style={[CallListStyle.cell, CallListStyle.titleCell]} numberOfLines={1}>
                {titleDisplay}
            </Text>
            <Text style={CallListStyle.cell}>{statusDisplay}</Text>
        </View>
    );
  };

  const renderPagination = () => {
    if (calls.length === 0) return null;

    let pages = [];
    
    // Lógica para definir qual o primeiro e o último botão numérico a ser exibido
    // Queremos mostrar no máximo MAX_VISIBLE_PAGES (4) botões
    let startPage, endPage;

    if (totalPages <= MAX_VISIBLE_PAGES) {
      // Se tiver menos de 4 páginas no total, mostra todas
      startPage = 1;
      endPage = totalPages;
    } else {
      // Se tiver muitas páginas, fazemos uma "janela" que se move
      if (currentPage <= Math.ceil(MAX_VISIBLE_PAGES / 2)) {
         // Se estiver no começo (ex: pág 1 ou 2), mostra [1, 2, 3, 4]
         startPage = 1;
         endPage = MAX_VISIBLE_PAGES;
      } else if (currentPage + Math.floor(MAX_VISIBLE_PAGES / 2) >= totalPages) {
         // Se estiver no final, mostra as últimas 4 (ex: [7, 8, 9, 10])
         startPage = totalPages - MAX_VISIBLE_PAGES + 1;
         endPage = totalPages;
      } else {
         // Se estiver no meio, centraliza (ex: pág 5 -> mostra [4, 5, 6, 7])
         startPage = currentPage - Math.floor(MAX_VISIBLE_PAGES / 2);
         endPage = startPage + MAX_VISIBLE_PAGES - 1;
      }
    }

    // Botão "Anterior" (<)
    pages.push(
      <TouchableOpacity
        key="prev"
        style={[CallListStyle.pageButton, currentPage === 1 && { opacity: 0.5 }]}
        disabled={currentPage === 1}
        onPress={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
      >
        <Ionicons name="chevron-back" size={20} color="#333" />
      </TouchableOpacity>
    );

    // Botões Numéricos
    for (let i = startPage; i <= endPage; i++) {
      pages.push(
        <TouchableOpacity
          key={i}
          style={[
            CallListStyle.pageButton,
            currentPage === i && CallListStyle.pageButtonActive,
          ]}
          onPress={() => setCurrentPage(i)}
        >
          <Text style={[
              CallListStyle.pageButtonText,
              currentPage === i && CallListStyle.pageButtonTextActive,
          ]}>
            {i}
          </Text>
        </TouchableOpacity>
      );
    }

    // Botão "Próximo" (>)
    pages.push(
      <TouchableOpacity
        key="next"
        style={[CallListStyle.pageButton, currentPage === totalPages && { opacity: 0.5 }]}
        disabled={currentPage === totalPages}
        onPress={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
      >
        <Ionicons name="chevron-forward" size={20} color="#333" />
      </TouchableOpacity>
    );

    return pages;
  };

  return (
    <SafeAreaView style={CallListStyle.container}>
      <Header showBackButton={true} onBackPress={() => navigation.goBack()} />
      <View style={CallListStyle.content}>
        <Text style={CallListStyle.title}>Meus Chamados</Text>
        {loading ? (
           <ActivityIndicator size="large" color="#00BFFF" style={{ marginTop: 20 }} />
        ) : (
          <>
            <View style={CallListStyle.tableContainer}>
              <View style={CallListStyle.tableHeader}>
                <Text style={CallListStyle.headerCell}>Ver</Text>
                <Text style={CallListStyle.headerCell}>ID</Text>
                <Text style={[CallListStyle.headerCell, CallListStyle.titleHeader]}>Título</Text>
                <Text style={CallListStyle.headerCell}>Status</Text>
              </View>
              {/* ATENÇÃO: data agora usa currentData (fatiado), não calls (tudo) */}
              <FlatList
                data={currentData} 
                renderItem={renderCallRow}
                keyExtractor={(item) => (item.Id_Chamado || item.id || Math.random()).toString()}
                style={CallListStyle.tableBody}
                ListEmptyComponent={<Text style={{padding: 20, textAlign: 'center'}}>Nenhum chamado encontrado.</Text>}
              />
            </View>
            
            {/* Só exibe paginação se houver chamados */}
            {calls.length > 0 && (
                <View style={CallListStyle.pagination}>{renderPagination()}</View>
            )}
          </>
        )}
      </View>
    </SafeAreaView>
  );
};
export default CallListScreen;